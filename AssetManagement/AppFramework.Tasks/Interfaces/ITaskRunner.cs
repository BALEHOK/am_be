using AppFramework.Tasks.Models;

namespace AppFramework.Tasks
{
    public interface ITaskRunner
    {
        TaskResult Run(Entities.Task task);
    }
}
