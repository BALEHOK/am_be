using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetManager.Infrastructure.Services
{
    public class TasksService : ITasksService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TasksService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public void SaveTask(Task task, long userId)
        {
            if (!task.IsActive && task.TaskId != 0)
            {
                _unitOfWork.TaskRepository.Delete(task);
                _unitOfWork.Commit();
                return;
            }

            task.UpdateUserId = userId;
            task.UpdateDate = DateTime.Now;

            if (task.TaskId == 0)
                _unitOfWork.TaskRepository.Insert(task);
            else
                _unitOfWork.TaskRepository.Update(task);
            _unitOfWork.Commit();
        }

        public Task GetTaskById(long id)
        {
            var task = _unitOfWork.TaskRepository.SingleOrDefault(t => t.TaskId == id);
            if (task == null)
                throw new EntityNotFoundException();
            return task;
        }

        public int GetCountByAssetTypeId(long atId)
        {
            return _unitOfWork.TaskRepository
                .AsQueryable()
                .Count(t => t.DynEntityConfigId == atId);
        }

        public IEnumerable<Task> GetByAssetTypeId(long atId)
        {
            return _unitOfWork.TaskRepository
                .Where(t => t.DynEntityConfigId == atId && t.IsActive);
        }

        public IEnumerable<ActiveTask> GetActive()
        {
            return _unitOfWork.GetTasks();
        }
    }
}
