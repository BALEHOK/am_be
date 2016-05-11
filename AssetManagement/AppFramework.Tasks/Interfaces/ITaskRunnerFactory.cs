namespace AppFramework.Tasks
{
    public interface ITaskRunnerFactory
    {
        ITaskRunner GetRunner(Entities.Task task, long currentUserId, long? dynEntityUid = null);
    }
}