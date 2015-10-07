using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Services;
using Microsoft.Practices.Unity;

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
            TaskFunctionType fType = (TaskFunctionType)Convert.ToInt32(input);
            string result = string.Empty;

            switch (fType)
            {
                case TaskFunctionType.CreateAsset:
                    result = "Create Asset";
                    break;
                case TaskFunctionType.ExecuteSearch:
                    result = "Execute Search";
                    break;
                case TaskFunctionType.ExportFileSearch:
                    result = "File Export (via search)";
                    break;
                case TaskFunctionType.ExportFileSSIS:
                    result = "File Export (SSIS)";
                    break;
                case TaskFunctionType.ImportFile:
                    result = "File Import";
                    break;
                case TaskFunctionType.LaunchBatch:
                    result = "Launch Batch Job";
                    break;
                case TaskFunctionType.PrintReport:
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
            long taskId = long.Parse(((LinkButton)sender).CommandArgument.ToString());
            var toDisable = TasksService.GetTaskById(taskId);
            toDisable.IsActive = !toDisable.IsActive;
            TasksService.SaveTask(toDisable, AuthenticationService.CurrentUserId);
            gvTasks.DataBind();
        }
    }
}