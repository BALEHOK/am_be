namespace AppFramework.Tasks.Runners
{
    using Common.Logging;
    using DataProxy;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    public class PredefinedTaskRunner
    {
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
        }

        private string _taskId;
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private List<string> _errors = new List<string>();
        private readonly IUnitOfWork _unitOfWork;

        public PredefinedTaskRunner(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
                throw new ArgumentNullException();
            _taskId = taskId;
            _unitOfWork = new UnitOfWork();
        }

        public TaskResult Run()
        {
            var result = new TaskResult(Enumerations.TaskFunctionType.Other);
            try
            {
                if (_taskId == PredefinedTasks.SobImportPayments)
                {
                    var importFolder = ConfigurationManager.AppSettings["SOBPaymentImportFolder"];
                    var importHistoryFolder = ConfigurationManager.AppSettings["SOBPaymentImportHistoryFolder"];

                    if (string.IsNullOrEmpty(importFolder) || !Directory.Exists(importFolder))
                        throw new DirectoryNotFoundException("Import folder is not defined. Please check config file and define following setting: <add key=\"SOBPaymentImportFolder\" value=\"your path\" /> ");
                    if (string.IsNullOrEmpty(importHistoryFolder) || !Directory.Exists(importHistoryFolder))
                        throw new DirectoryNotFoundException("Import History folder is not defined. Please check config file and define following setting: <add key=\"SOBPaymentImportHistoryFolder\" value=\"your path\" /> ");

                    new SobImportPayments(
                            _unitOfWork, 
                            GetSOBCertificateConfigUid(), 
                            GetPaymentConfigUid(),
                            importFolder,
                            importHistoryFolder)
                        .DoImport();
                }
                result.Status = Enumerations.TaskStatus.Sussess;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                result.Errors.Add(ex.Message);
                result.Status = Enumerations.TaskStatus.Error;
            }
            return result;
        }

        private long GetPaymentConfigUid()
        {
            object res =
                _unitOfWork.SqlProvider.ExecuteScalar(
                    "SELECT DynEntityConfigUid FROM DynEntityConfig WHERE Name = 'SOBPayment' AND ActiveVersion = 1");
            return (long)res;
        }

        long GetSOBCertificateConfigUid()
        {
            object res = _unitOfWork.SqlProvider.ExecuteScalar("SELECT DynEntityConfigUid FROM DynEntityConfig WHERE Name = 'SOBCertificate' AND ActiveVersion = 1");
            return (long)res;
        }
    }
}
