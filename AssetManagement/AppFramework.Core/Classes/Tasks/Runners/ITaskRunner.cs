namespace AppFramework.Core.Classes.Tasks.Runners
{
    public interface ITaskRunner
    {
        TaskResult Run(Entities.Task task);
    }
}
