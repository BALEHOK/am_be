using System.Collections.Generic;

namespace AppFramework.Tasks.Models
{
    public class TaskResult
    {
        public Enumerations.TaskFunctionType TaskFunctionType { get; private set; }

        public Enumerations.TaskStatus Status { get; set; }

        public List<string> Errors { get; set; }
     
        public string NavigationResult { get; set; }

        public Dictionary<string, object> NavigationResultArguments { get; set; }

        public Enumerations.TaskActionOnComplete ActionOnComplete
        {
            get
            {
                var result = Enumerations.TaskActionOnComplete.Nothing;
                switch (TaskFunctionType)
                {
                    case Enumerations.TaskFunctionType.ExecuteSearch:
                    case Enumerations.TaskFunctionType.CreateAsset:
                    case Enumerations.TaskFunctionType.PrintReport:
                        result = Enumerations.TaskActionOnComplete.Navigate;
                        break;

                    default:
                        result = Enumerations.TaskActionOnComplete.Nothing;
                        break;
                }
                return result;
            }
        }

        public TaskResult(Enumerations.TaskFunctionType taskFunctionType)
        {
            TaskFunctionType = taskFunctionType;
            Errors = new List<string>();
            NavigationResultArguments = new Dictionary<string, object>();
            Status = Enumerations.TaskStatus.Sussess;
        }
    }
}
