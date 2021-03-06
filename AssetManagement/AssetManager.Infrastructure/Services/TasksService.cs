﻿using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.Tasks;
using AppFramework.Tasks.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public Task GetTaskById(long id, long userId)
        {
            if (!IsTaskPermitted(id, userId))
            {
                throw new EntityNotFoundException();
            }

            var task = _unitOfWork.TaskRepository.SingleOrDefault(t => t.TaskId == id);
            if (task == null)
                throw new EntityNotFoundException();
            return task;
        }

        [Obsolete("Method doesn't check access rights. Update it or do not use.")]
        public int GetCountByAssetTypeId(long atId)
        {
            return _unitOfWork.TaskRepository
                .AsQueryable()
                .Count(t => t.DynEntityConfigId == atId);
        }

        public IEnumerable<Task> GetByAssetTypeId(long atId, long userId)
        {
            return FilterPermitted(_unitOfWork.TaskRepository
                .Where(t => t.DynEntityConfigId == atId && t.IsActive), userId);
        }

        public IEnumerable<ActiveTask> GetActive(long userId)
        {
            // user access checked in SP
            return _unitOfWork.GetTasks(userId);
        }

        public IEnumerable<PredefinedTaskModel> GetPredefinedTasks()
        {
            var allowedTasks = new string[] { };
            var config = ConfigurationManager.AppSettings["PredefinedTasks"];
            if (!string.IsNullOrEmpty(config))
            {
                allowedTasks = config.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            return PredefinedTasks.Tasks
                .Where(t => allowedTasks.Contains(t.TaskId));
        }

        private bool IsTaskPermitted(long taskId, long userId)
        {
            var permittedTasks = _unitOfWork.GetPermittedTasks(userId).ToList();

            return permittedTasks.Any(tId => tId == taskId);
        }

        private IEnumerable<Task> FilterPermitted(IEnumerable<Task> tasks, long userId)
        {
            var permittedTasks = _unitOfWork.GetPermittedTasks(userId).ToList();
            return tasks.Where(t => permittedTasks.Any(id => id == t.TaskId));
        }
    }
}
