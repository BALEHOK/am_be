namespace AppFramework.Core.Classes.Tasks.Runners
{
    public interface ITaskRunnerFactory
    {
        ITaskRunner GetRunner(Entities.Task task, long currentUserId, long? dynEntityUid);
    }
}