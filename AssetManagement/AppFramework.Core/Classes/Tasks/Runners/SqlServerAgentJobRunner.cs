using AppFramework.DataProxy;
using AppFramework.Tasks;
using AppFramework.Tasks.Models;
using Common.Logging;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AppFramework.Core.Classes.Tasks.Runners
{
    internal class SqlServerAgentJobRunner : ITaskRunner
    {
        private readonly string _jobId;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public SqlServerAgentJobRunner(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException();
            _unitOfWork = unitOfWork;
        }

        public TaskResult Run(Entities.Task task)
        {
            if (task == null)
                throw new ArgumentNullException();
            if (string.IsNullOrEmpty(task.FunctionData))
                throw new ArgumentException("Cannot retrieve job_id from task data");

            var result = new TaskResult((Enumerations.TaskFunctionType)task.FunctionType);
            try
            {
                var guid = Guid.Parse(task.FunctionData);
                _logger.DebugFormat("Executing sp_start_job with job_id = {0} ", guid);
                _unitOfWork.SqlProvider.ExecuteNonQuery("msdb.dbo.sp_start_job", new SqlParameter[]
                {
                    new SqlParameter("@job_id", guid) {SqlDbType = SqlDbType.UniqueIdentifier}
                }, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                result.Errors.Add(ex.Message);
                result.Status = Enumerations.TaskStatus.Error;
            }
            return result;
        }
    }
}
