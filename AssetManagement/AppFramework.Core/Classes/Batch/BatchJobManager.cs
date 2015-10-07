using AppFramework.ConstantsEnumerators;
using AppFramework.DataProxy;
using System;
using System.Linq;

namespace AppFramework.Core.Classes.Batch
{
    public class BatchJobManager : IBatchJobManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBatchActionFactory _batchActionFactory;

        public BatchJobManager(IUnitOfWork unitOfWork, IBatchActionFactory batchActionFactory)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _unitOfWork = unitOfWork;
            if (batchActionFactory == null)
                throw new ArgumentNullException("batchActionFactory");
            _batchActionFactory = batchActionFactory;
        }

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        public void AddAction(BatchJob job, BatchActionType actionType, BatchActionParameters @params, bool isMandatory = true)
        {
            long lastAction = job.GetActions().IsNullOrEmpty() ? 0 : job.GetActions().Max(a => a.Order);
            var newAction = new Entities.BatchAction()
            {
                BatchUid = job.UID,
                ActionType = (int)actionType,
                OrderId = lastAction + 1,
                Status = (short)BatchStatus.Created,
                ErrorMessage = string.Empty,
                ActionParams = @params.ToXml(),
                IsMandatory = isMandatory
            };
            job.Base.BatchActions.Add(newAction);
            job.GetActions().Add(_batchActionFactory.GetAction(newAction));
        }

        /// <summary>
        /// Permanently deletes the finished jobs
        /// </summary>
        public void DeleteAllJobs()
        {
            var finished = _unitOfWork.BatchJobRepository.Get(
                b => b.BatchSchedule == null
                );
            foreach (var job in finished)
            {
                _unitOfWork.BatchJobRepository.Delete(job);
            }
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Permanently deletes the finished jobs
        /// </summary>
        public void DeleteFinishedJobs()
        {
            var finished = _unitOfWork.BatchJobRepository.Get(
                    b => b.Status == (short)BatchStatus.Finished &&
                    b.BatchSchedule == null
                );
            foreach (var job in finished)
            {
                _unitOfWork.BatchJobRepository.Delete(job);
            }
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Returns job instance by its UID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public BatchJob GetByUid(long uid)
        {
            var ib = new IncludesBuilder<Entities.BatchJob>();
            ib.Add(b => b.BatchActions);
            ib.Add(b => b.BatchSchedule);
            var dbJob = _unitOfWork.BatchJobRepository.SingleOrDefault(b => b.BatchUid == uid, ib.Get());
            return new BatchJob(dbJob, _batchActionFactory);
        }

        /// <summary>
        /// Saves job instance
        /// </summary>
        /// <param name="job"></param>
        public void SaveJob(BatchJob job)
        {
            if (job.Base.BatchUid > 0)
            {
                _unitOfWork.BatchJobRepository.Update(job.Base);
            }
            else
            {
                _unitOfWork.BatchJobRepository.Insert(job.Base);
            }
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Mark job as enqueued
        /// </summary>
        public void StackForExecution(BatchJob job)
        {
            job.Base.Status = (short)BatchStatus.InStack;
            job.Base.EndDate = job.Base.StartDate = null;
            SaveJob(job);
        }
    }
}
