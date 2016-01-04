using System.Collections.Generic;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Services
{
    public interface ITasksService
    {
        IEnumerable<Task> GetByAssetTypeId(long atId, long userId);

        int GetCountByAssetTypeId(long atId);

        Task GetTaskById(long id, long userId);

        void SaveTask(Task task, long userId);

        IEnumerable<ActiveTask> GetActive(long userId);
    }
}