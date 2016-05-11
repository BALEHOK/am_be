using AppFramework.Tasks;
using AppFramework.Tasks.Models;

namespace AppFramework.Core.Classes.Tasks.Runners
{
    class SearchRunner : ITaskRunner
    {
        public virtual TaskResult Run(Entities.Task task)
        {
            return new TaskResult((Enumerations.TaskFunctionType)task.FunctionType)
            {
                Status = Enumerations.TaskStatus.Sussess,
                NavigationResult = task.FunctionData
            };
        }
    }
}
