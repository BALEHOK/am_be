using System;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.Core.Classes.Extensions;
using AssetManager.Infrastructure.Extensions;
using AppFramework.Tasks.Models;
using AppFramework.Tasks;
using AppFramework.Tasks.Runners;

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
            var tasks = _tasksService.GetActive(User.GetId());
            var predefined = _tasksService.GetPredefinedTasks();
            return tasks.Select(t => new TaskModel
            {
                Id = t.TaskId,
                Name = t.Name.Localized(),
                Description = t.Description,
                DynEntityConfigId = t.DynEntityConfigId,
                DynEntityConfigName = t.DynEntityConfigName.Localized()
            })
            .Union(predefined.Select(t => new TaskModel
            {
                Id = t.TaskId,
                Name = t.Name.Localized(),
                Description = t.Description,
                IsPredefined = true
            }));
        }

        [Route("assettype/{assetTypeId}")]
        public IEnumerable<TaskModel> GetTasksByAssetType(long assetTypeId)
        {
            var tasks = _tasksService.GetByAssetTypeId(assetTypeId, User.GetId())
                .Where(t => t.DisplayInSidebar);
            return tasks.Select(t => new TaskModel
            {
                Id = t.TaskId,
                Name = t.Name.Localized()
            });
        }

        [Route("{taskId}/execute-predefined"), HttpPost]
        public TaskResultModel ExecutePredefinedTask(string taskId)
        {
            var runner = new PredefinedTaskRunner(taskId);
            var result = runner.Run();
            return new TaskResultModel
            {
                TaskFunctionType = result.TaskFunctionType.ToString().ToUpper(), // string constants to attach frontend logic
                ShouldRedirectOnComplete = result.ActionOnComplete == Enumerations.TaskActionOnComplete.Navigate, // void or should navigate
                Status = result.Status.ToString().ToUpper(), // SUCCESS or ERROR
                Errors = result.Errors, // array or errors 
                TaskId = taskId
            };
        }

        [Route("{taskId}/execute"), HttpPost]
        public TaskResultModel ExecuteRegularTask(long taskId)
        {
            var userId = User.GetId();
            var task = _tasksService.GetTaskById(taskId, userId);
            var runner = _taskRunnerFactory.GetRunner(task, userId);
            var result = runner.Run(task);

            dynamic navigationResult;
            if ((Enumerations.TaskFunctionType)task.FunctionType == Enumerations.TaskFunctionType.ExecuteSearch)
            {
                navigationResult = result.NavigationResult;
            }
            else
            {
                navigationResult = result.NavigationResultArguments;
            }

            return new TaskResultModel
            {
                TaskFunctionType = result.TaskFunctionType.ToString().ToUpper(), // string constants to attach frontend logic
                ShouldRedirectOnComplete = result.ActionOnComplete == Enumerations.TaskActionOnComplete.Navigate, // void or should navigate
                Result = navigationResult, // array of arguments to build redirect url
                Status = result.Status.ToString().ToUpper(), // SUCCESS or ERROR
                Errors = result.Errors, // array or errors 
                TaskName = task.Name.Localized(),
                TaskId = task.TaskId
            };
        }
    }
}
