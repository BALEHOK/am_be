using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Tasks.Runners
{
    using AppFramework.Core.Classes.Tasks;
    using AppFramework.Tasks;
    using System;
    using System.Collections.Generic;

    public class TaskRunnerFactory : ITaskRunnerFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public TaskRunnerFactory(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
        }
                
        public ITaskRunner GetRunner(Entities.Task task, long currentUserId, long? dynEntityUid)
        {
            if (task == null)
                throw new ArgumentNullException("Task");

            ITaskRunner runner = null;
            if (task.ExecutableType == (int)Enumerations.TaskExecutableType.SSIS)
            {
                runner = new SSISRunner(task.ExecutablePath);
                SetParameters((runner as SSISRunner).Parameters, task);
                SetContext((runner as SSISRunner).Parameters, task, currentUserId, dynEntityUid);
            }
            else if (task.ExecutableType == (int)Enumerations.TaskExecutableType.Assembly)
            {
                runner = new ProcessRunner();
                (runner as ProcessRunner).ExecutablePath = task.ExecutablePath;
                SetParameters((runner as ProcessRunner).Parameters, task);

            }
            else if (task.ExecutableType == (int)Enumerations.TaskExecutableType.Internal)
            {
                switch ((Enumerations.TaskFunctionType)task.FunctionType)
                {
                    case Enumerations.TaskFunctionType.ExecuteSearch:
                        runner = new SearchRunner();
                        break;
                                         
                    case Enumerations.TaskFunctionType.CreateAsset:
                        runner = new AssetCreationRunner(_assetTypeRepository, _unitOfWork);
                        if (!string.IsNullOrEmpty(task.FunctionData))
                        {
                            var descriptor = NewAssetTaskParametrsDescriptor.Deserialize(task.FunctionData);
                            (runner as AssetCreationRunner).ScreenId = descriptor.ScreenId;
                            (runner as AssetCreationRunner).DynEntityConfigId = task.DynEntityConfigId;
                        }
                        break;

                    case Enumerations.TaskFunctionType.PrintReport:
                        runner = new PrintReportRunner(task.FunctionData, dynEntityUid ?? 0);
                        break;

                    case Enumerations.TaskFunctionType.ExportFileSSIS:
                    case Enumerations.TaskFunctionType.LaunchBatch:
                    case Enumerations.TaskFunctionType.ImportFile:
                        throw new ArgumentException("Wrong FunctionType for task");

                    case Enumerations.TaskFunctionType.ExecuteSqlServerAgentJob:
                        runner = new SqlServerAgentJobRunner(_unitOfWork);
                        break;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            if (runner != null)
            {
                return runner;
            }
            else
            {
                throw new NotImplementedException("This task type is not implemented");
            }
        }

        private void SetParameters(Dictionary<string, string> dicparameters, Entities.Task task)
        {
            if (string.IsNullOrEmpty(task.FunctionData)) return;
            ParametersDescriptor parameters;
            switch ((Enumerations.TaskFunctionType)task.FunctionType)
            {
                case Enumerations.TaskFunctionType.ImportFile:
                    parameters = ImportTaskParametersDescriptor.Deserialize(task.FunctionData);
                    break;
                case Enumerations.TaskFunctionType.ExportFileSSIS:
                    parameters = ExportTaskParametersDescriptor.Deserialize(task.FunctionData);
                    break;
                default:
                    parameters = BatchTaskParametersDescriptor.Deserialize(task.FunctionData);
                    break;
            }

            foreach (var param in parameters.Data)
            {
                dicparameters.Add(param.Key, param.Value);
            }
        }

        private void SetContext(
            Dictionary<string, string> dictionary, 
            Entities.Task task, 
            long currentUserId,
            long? dynEntityUid)
        {
            if (dynEntityUid.HasValue)
            {
                dictionary.Add("CurrentDynEntityUid", dynEntityUid.Value.ToString());
                dictionary.Add("CurrentDynEntityConfigId", task.DynEntityConfigId.ToString());
            }
            dictionary.Add("CurrentUserId", currentUserId.ToString());
        }
    }
}
