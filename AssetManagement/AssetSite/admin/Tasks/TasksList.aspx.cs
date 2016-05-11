using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Services;
using Microsoft.Practices.Unity;
using AppFramework.Tasks;

namespace AssetSite.admin.Tasks
{
    public partial class TasksList : BasePage
    {
        [Dependency]
        public ITasksService TasksService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["AssetTypeId"]))
            {
                hplCreateTask.NavigateUrl = "EditTask.aspx?AssetTypeId=" + Request.QueryString["AssetTypeId"];
            }
            else
            {
                Response.Redirect("~/Wizard/EditAssetType.aspx");
            }
        }

        public string GetFunctionType(object input)
        {
            Enumerations.TaskFunctionType fType = (Enumerations.TaskFunctionType)Convert.ToInt32(input);
            string result = string.Empty;

            switch (fType)
            {
                case Enumerations.TaskFunctionType.CreateAsset:
                    result = "Create Asset";
                    break;
                case Enumerations.TaskFunctionType.ExecuteSearch:
                    result = "Execute Search";
                    break;
                case Enumerations.TaskFunctionType.ExportFileSSIS:
                    result = "File Export (SSIS)";
                    break;
                case Enumerations.TaskFunctionType.ImportFile:
                    result = "File Import";
                    break;
                case Enumerations.TaskFunctionType.LaunchBatch:
                    result = "Launch Batch Job";
                    break;
                case Enumerations.TaskFunctionType.PrintReport:
                    result = "Print Report";
                    break;
            }

            return result;
        }

        protected string GetEditUrl(object taskID)
        {
            return "EditTask.aspx?AssetTypeId=" + Request.QueryString["AssetTypeId"] + "&TaskId=" + taskID + "&Edit=1";
        }

        protected void gvTasks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var task = e.Row.DataItem as AppFramework.Entities.Task;
                var translation = new TranslatableString(task.Name).GetTranslation();   // TODO: move to enity level via partial class?
                (e.Row.Cells[0].FindControl("TranslatedName") as Literal).Text = translation;

                var descrTranslation = new TranslatableString(task.Description).GetTranslation();
                (e.Row.Cells[0].FindControl("TranslatedDescription") as Literal).Text = descrTranslation;

                var deleteButton = e.Row.FindControl("lbtnMakeInactive") as LinkButton;
                string postbackEvent = Page.ClientScript.GetPostBackEventReference(deleteButton, "");
                deleteButton.OnClientClick = "return ShowConfirmationDialog(function(){ " + postbackEvent + " });";
            }
        }

        protected void lbtnMakeInactive_Click(object sender, EventArgs e)
        {
            long taskId = long.Parse(((LinkButton)sender).CommandArgument);
            var toDisable = TasksService.GetTaskById(taskId, AuthenticationService.CurrentUserId);
            toDisable.IsActive = !toDisable.IsActive;
            TasksService.SaveTask(toDisable, AuthenticationService.CurrentUserId);
            gvTasks.DataBind();
        }
    }
}