using AppFramework.ConstantsEnumerators;
using System.Collections.Generic;

namespace AppFramework.Core.Classes.Tasks
{
    public class TaskResult
    {
        public TaskFunctionType TaskFunctionType { get; private set; }

        public TaskStatus Status { get; set; }

        public List<string> Errors { get; set; }
     
        public string NavigationResult { get; set; }

        public Dictionary<string, object> NavigationResultArguments { get; set; }

        public TaskActionOnComplete ActionOnComplete
        {
            get
            {
                var result = TaskActionOnComplete.Nothing;
                switch (TaskFunctionType)
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
            TaskFunctionType = taskFunctionType;
            Errors = new List<string>();
            NavigationResultArguments = new Dictionary<string, object>();
            Status = TaskStatus.Sussess;
        }
    }
}
