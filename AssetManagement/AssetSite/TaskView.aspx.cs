using System;
using AppFramework.Core.Classes;
using Microsoft.Practices.Unity;
using AssetManager.Infrastructure.Services;
using AppFramework.Core.Classes.Tasks.Runners;
using AssetManager.Infrastructure.Extensions;
using AppFramework.Tasks;

namespace AssetSite
{
    public partial class TaskView : BasePage
    {
        [Dependency]
        public ITasksService TasksService { get; set; }
        [Dependency]
        public ITaskRunnerFactory TaskRunnerFactory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            assetTreeView.AssetTypeSelectedEvent += assetTreeView_AssetTypeSelectedEvent;
        }

        protected void assetTreeView_AssetTypeSelectedEvent(AssetType assetType, TaxonomyItem item)
        {
            if (UnitOfWork.GetPermittedTask(assetType.ID, AuthenticationService.CurrentUserId, item.Id))
            {
                lbltaskListEmpty.Visible = false;
                repTasks.Visible = true;
                repTasks.DataSource = TasksService.GetByAssetTypeId(assetType.ID, AuthenticationService.CurrentUserId);
                repTasks.DataBind();
            }
            else
            {
                repTasks.Visible = false;
                lbltaskListEmpty.Visible = true;                
            }            
        }
    }
}