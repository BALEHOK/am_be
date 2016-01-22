using System;
using System.Linq;
using System.Transactions;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Batch;
using AppFramework.DataProxy;
using Common.Logging;
using Quartz;

namespace AssetManagementBatch.Jobs
{
    [DisallowConcurrentExecution]
    public class RunBatchJob : IJob
    {
        private readonly ILog _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBatchActionFactory _batchActionFactory;

        public RunBatchJob(IUnitOfWork unitOfWork, IBatchActionFactory batchActionFactory, ILog logger)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (batchActionFactory == null)
                throw new ArgumentNullException("batchActionFactory");
            _batchActionFactory = batchActionFactory;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        public void Execute(IJobExecutionContext context)
        {
            AppFramework.Entities.BatchJob job = null;
            try
            {
                job = DequeueJob();
                if (job != null)
                {
                    // log job start
                    _logger.DebugFormat("Starting job: \"{0}\" (uid {1}).{2}",
                        job.Title,
                        job.BatchUid,
                        (job.BatchScheduleId.HasValue && job.BatchSchedule.IsEnabled)
                            ? string.Format(" Scheduled on {0}{1}.",
                                job.BatchSchedule.ExecuteAt,
                                job.BatchSchedule.RepeatInHours > 0
                                    ? string.Format(" with repetition each {0} hours (last start at {1})",
                                        job.BatchSchedule.RepeatInHours, job.BatchSchedule.LastStart)
                                    : string.Empty)
                            : string.Empty);

                    // execute job
                    ExecuteJob(job);

                    // log job completion
                    _logger.InfoFormat("Job \"{0}\" (uid {1}) completed with status {2}.", job.Title, job.BatchUid,
                        ((BatchStatus)job.Status).ToString());
                }
            }
            catch (Exception ex)
            {
                if (job != null)
                    _logger.DebugFormat("Error has occured during job execution (\"{0}\", uid {1})",
                        job.Title, job.BatchUid);
                _logger.Error(ex);
            }
        }

        public AppFramework.Entities.BatchJob DequeueJob()
        {
            // pick a pending job            
            var ib = new IncludesBuilder<AppFramework.Entities.BatchJob>();

            ib.Add(b => b.BatchActions);
            ib.Add(b => b.BatchSchedule);

            var jobs = _unitOfWork.BatchJobRepository
                .Where(b => b.Status == (int)BatchStatus.InStack, ib.Get())
                .Take(1)
                .ToList();

            if (jobs.Count() == 1)
                return jobs[0];

            // check if there are any pending scheduled jobs
            var scheduledJobs = _unitOfWork.BatchJobRepository
                .Where(b => b.BatchScheduleId != null && b.BatchSchedule.IsEnabled &&
                            (b.Status == (int)BatchStatus.InStack ||
                             b.Status == (int)BatchStatus.Created ||
                             b.Status == (int)BatchStatus.Finished ||
                             b.Status == (int)BatchStatus.FinishedWithErrors),
                    ib.Get());

            foreach (var sj in scheduledJobs)
            {
                if (sj.BatchSchedule.LastStart == null) // execute job firstly...
                {
                    // if elapsed
                    if (sj.BatchSchedule.ExecuteAt <= DateTime.Now)
                        return sj;
                }
                else if (sj.BatchSchedule.RepeatInHours > 0) // execute schedule job...
                {
                    // if elapsed
                    if (sj.BatchSchedule.LastStart.Value.AddHours(sj.BatchSchedule.RepeatInHours.Value) <= DateTime.Now)
                        return sj;
                }
            }
            return null;
        }

        public void ExecuteJob(AppFramework.Entities.BatchJob job)
        {
            // update status
            job.Status = (short)BatchStatus.Running;
            job.StartDate = DateTime.Now;
            _unitOfWork.BatchJobRepository.Update(job);
            _unitOfWork.Commit();

            var actions = (from a in job.BatchActions.OrderBy(a => a.OrderId)
                           select _batchActionFactory.GetAction(a)).ToList();

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };

            bool allActionsOk;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                // run actions
                foreach (var action in actions)
                {
                    action.Execute(_unitOfWork);

                    if (action.Status == BatchStatus.Error && job.SkipErrors == false)
                    {
                        for (int i = actions.IndexOf(action) + 1; i < actions.Count; i++)
                        {
                            actions[i].Status = BatchStatus.Skipped;
                        }
                        break;
                    }
                }

                _unitOfWork.Commit();

                allActionsOk = actions.All(r => r.Status == BatchStatus.Finished);
                if (allActionsOk)
                    scope.Complete();
            }

            // update status
            job.EndDate = DateTime.Now;

            if (allActionsOk)
                job.Status = (int)BatchStatus.Finished;
            else if (actions.Any(a => a.Status == BatchStatus.Error) &&
                     actions.Any(a => a.Status == BatchStatus.Finished) && job.SkipErrors)
                job.Status = (int)BatchStatus.FinishedWithErrors;
            else
                job.Status = (int)BatchStatus.Error;

            if (job.BatchScheduleId != null && job.BatchSchedule.IsEnabled)
            {
                job.BatchSchedule.LastStart = job.EndDate;
                _unitOfWork.BatchScheduleRepository.Update(job.BatchSchedule);
            }

            _unitOfWork.BatchJobRepository.Update(job);
            _unitOfWork.Commit();
        }
    }
}
