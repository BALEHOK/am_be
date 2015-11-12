using System;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.Core.Classes.Tasks.Runners;
using AssetManager.WebApi.Extensions;
using AppFramework.ConstantsEnumerators;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/tasks")]
    public class TasksController : ApiController
    {
        private readonly ITasksService _tasksService;
        private readonly ITaskRunnerFactory _taskRunnerFactory;

        public TasksController(
            ITasksService tasksService,
            ITaskRunnerFactory taskRunnerFactory)
        {
            if (tasksService == null)
                throw new ArgumentNullException("tasksService");
            _tasksService = tasksService;
            if (taskRunnerFactory == null)
                throw new ArgumentNullException("taskRunnerFactory");
            _taskRunnerFactory = taskRunnerFactory;
        }

        [Route("")]
        public IEnumerable<TaskModel> GetTasks()
        {
            var tasks = _tasksService.GetActive();
            return tasks.Select(t => new TaskModel
            {
                Id = t.TaskId,
                Name = t.Name,
                Description = t.Description,
                DynEntityConfigId = t.DynEntityConfigId,
                DynEntityConfigName = t.DynEntityConfigName
            });
        }

        [Route("assettype/{assetTypeId}")]
        public IEnumerable<TaskModel> GetTasksByAssetType(long assetTypeId)
        {
            var tasks = _tasksService.GetByAssetTypeId(assetTypeId)
                .Where(t => t.DisplayInSidebar);
            return tasks.Select(t => new TaskModel
            {
                Id = t.TaskId,
                Name = t.Name
            });
        }

        [Route("{taskId}/execute"), HttpPost]
        public TaskResultModel ExecuteTask(long taskId)
        {
            var task = _tasksService.GetTaskById(taskId);
            var userId = User.GetId();
            var runner = _taskRunnerFactory.GetRunner(task, userId);
            var result = runner.Run(task);

            return new TaskResultModel
            {
                TaskFunctionType = result.TaskFunctionType.ToString().ToUpper(), // string constants to attach frontend logic
                ShouldRedirectOnComplete = result.ActionOnComplete == TaskActionOnComplete.Navigate, // void or should navigate
                Result = result.NavigationResultArguments, // array of arguments to build redirect url
                Status = result.Status.ToString().ToUpper(), // SUCCESS or ERROR
                Errors = result.Errors, // array or errors 
                TaskName = task.Name,
                TaskId = task.TaskId              
            };
        }
    }
}
