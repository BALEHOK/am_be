using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Tasks.Runners
{
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes.Tasks;
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
            if (task.ExecutableType == (int)TaskExecutableType.SSIS)
            {
                runner = new SSISRunner(task.ExecutablePath);
                SetParameters((runner as SSISRunner).Parameters, task);
                SetContext((runner as SSISRunner).Parameters, task, currentUserId, dynEntityUid);
            }
            else if (task.ExecutableType == (int)TaskExecutableType.Assembly)
            {
                runner = new ProcessRunner();
                (runner as ProcessRunner).ExecutablePath = task.ExecutablePath;
                SetParameters((runner as ProcessRunner).Parameters, task);

            }
            else if (task.ExecutableType == (int)TaskExecutableType.PredefinedTask)
            {
                runner = new PredefinedTaskRunner(task.ExecutablePath);
                SetParameters((runner as PredefinedTaskRunner).Parameters, task);
                SetContext((runner as PredefinedTaskRunner).Parameters, task, currentUserId, dynEntityUid);    
            }
            else if (task.ExecutableType == (int)TaskExecutableType.Internal)
            {
                switch ((TaskFunctionType)task.FunctionType)
                {
                    case TaskFunctionType.ExecuteSearch:
                        runner = new SearchRunner();
                        if (!string.IsNullOrEmpty(task.FunctionData))
                        {
                            var searchParams = SearchConfigurationDescriptor.Deserialize(task.FunctionData);
                            (runner as SearchRunner).Params = searchParams;
                        }
                        break;

                    case TaskFunctionType.ExportFileSearch:
                        runner = new ExportRunner();
                        if (!string.IsNullOrEmpty(task.FunctionData))
                        {
                            var searchParams = SearchConfigurationDescriptor.Deserialize(task.FunctionData);
                            (runner as ExportRunner).Params = searchParams;
                        }
                        break;

                    case TaskFunctionType.CreateAsset:
                        runner = new AssetCreationRunner(_assetTypeRepository, _unitOfWork);
                        if (!string.IsNullOrEmpty(task.FunctionData))
                        {
                            var descriptor = NewAssetTaskParametrsDescriptor.Deserialize(task.FunctionData);
                            (runner as AssetCreationRunner).ScreenId = descriptor.ScreenId;
                            (runner as AssetCreationRunner).DynEntityConfigId = task.DynEntityConfigId;
                        }
                        break;

                    case TaskFunctionType.PrintReport:
                        runner = new PrintReportRunner(task.FunctionData, dynEntityUid ?? 0);
                        break;

                    case TaskFunctionType.ExportFileSSIS:
                    case TaskFunctionType.LaunchBatch:
                    case TaskFunctionType.ImportFile:
                        throw new ArgumentException("Wrong FunctionType for task");

                    case TaskFunctionType.ExecuteSqlServerAgentJob:
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
            switch ((TaskFunctionType)task.FunctionType)
            {
                case TaskFunctionType.ImportFile:
                    parameters = ImportTaskParametersDescriptor.Deserialize(task.FunctionData);
                    break;
                case TaskFunctionType.ExportFileSSIS:
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
