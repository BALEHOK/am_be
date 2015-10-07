using System;
using Common.Logging;

namespace AssetManagementBatch
{
    internal sealed class ExceptionOccuredJobListener : Quartz.IJobListener
    {
        private readonly ILog _logger;

        public ExceptionOccuredJobListener(ILog logger)
        {
            if (logger == null)
                throw new ArgumentNullException("ILog");
            _logger = logger;
        }

        public void JobToBeExecuted(Quartz.IJobExecutionContext context) { }

        public void JobExecutionVetoed(Quartz.IJobExecutionContext context)
        {
            _logger.WarnFormat("Job {0} vetoed",
                context.JobDetail.JobType);
        }

        public void JobWasExecuted(Quartz.IJobExecutionContext context,
                                   Quartz.JobExecutionException jobException)
        {
            if (jobException != null)
            {
                var exception = jobException.GetBaseException();
                _logger.ErrorFormat("unhandled exception occured in {0}.", exception,
                    context.JobDetail.JobType);
            }
        }

        public string Name
        {
            get
            {
                return "ExceptionOccuredJobListener";
            }
        }
    }
}
