
namespace AppFramework.Core.Classes.Tasks.Runners
{
    using AppFramework.ConstantsEnumerators;
    using Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using AppFramework.DataProxy;

    class PredefinedTaskRunner : ITaskRunner
    {
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
        }

        private string _packageLocation;
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private List<string> _errors = new List<string>();
        private readonly IUnitOfWork _unitOfWork;

        public PredefinedTaskRunner(string packageLocation)
        {
            if (string.IsNullOrEmpty(packageLocation))
                throw new ArgumentNullException();
            _packageLocation = packageLocation;
            _unitOfWork = new UnitOfWork();
        }

        public TaskResult Run(Entities.Task task)
        {
            var result = new TaskResult((TaskFunctionType)task.FunctionType);
            try
            {
                if (_packageLocation == "SobImportPayments")
                {
                    new SobImportPayments(_unitOfWork, 
                        GetSOBCertificateConfigUid(), 
                        GetPaymentConfigUid(),
                        ConfigurationManager.AppSettings["SOBPaymentImportFolder"],
                        ConfigurationManager.AppSettings["SOBPaymentImportHistoryFolder"])
                        .DoImport();
                    //Thread.Sleep(3000);
                }
                result.Status = TaskStatus.Sussess;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                result.Errors.Add(ex.Message);
                result.Status = TaskStatus.Error;
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
