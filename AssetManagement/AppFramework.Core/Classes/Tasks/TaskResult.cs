using AppFramework.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Core.Classes.Tasks
{
    public class TaskResult
    {
        private readonly TaskFunctionType _taskFunctionType;

        public AppFramework.ConstantsEnumerators.TaskStatus Status { get; set; }

        public List<string> Errors { get; set; }
     
        public string NavigationResult { get; set; }

        public TaskActionOnComplete ActionOnComplete
        {
            get
            {
                TaskActionOnComplete result = TaskActionOnComplete.Nothing;
                switch (_taskFunctionType)
                {
                    case TaskFunctionType.ExportFileSearch:
                    case TaskFunctionType.ExecuteSearch:
                    case TaskFunctionType.CreateAsset:
                    case TaskFunctionType.PrintReport:
                        result = TaskActionOnComplete.Navigate;
                        break;

                    default:
                        result = TaskActionOnComplete.Nothing;
                        break;
                }
                return result;
            }
        }

        public TaskResult(TaskFunctionType taskFunctionType)
        {
            _taskFunctionType = taskFunctionType;
            Errors = new List<string>();
            Status = AppFramework.ConstantsEnumerators.TaskStatus.Sussess;
        }
    }
}
