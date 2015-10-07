using System.Collections.Generic;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Services
{
    public interface ITasksService
    {
        IEnumerable<Task> GetByAssetTypeId(long atId);

        int GetCountByAssetTypeId(long atId);

        Task GetTaskById(long id);

        void SaveTask(Task task, long userId);
    }
}