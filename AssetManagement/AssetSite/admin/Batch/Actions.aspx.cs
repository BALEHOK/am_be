using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Batch;
using AppFramework.DataProxy;
using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.Batch
{
    public partial class Actions : BasePage
    {
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }

        protected BatchJob BatchJob { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            long uid = 0;
            if (Request["BatchUid"] != null && long.TryParse(Request["BatchUid"], out uid))
            {
                BatchJob = BatchJobManager.GetByUid(uid);

                if (BatchJob.CurrentStatus != BatchStatus.Running && Request["Execute"] == "1")
                {
                    BatchJobManager.StackForExecution(BatchJob);
                    Response.Redirect(string.Format("/admin/Batch/Actions.aspx?BatchUid={0}", uid));
                    return;
                }

                if (!IsPostBack)
                {
                    ActionsGrid.DataSource = BatchJob.GetActions();
                    ActionsGrid.DataBind();
                    JobStatus.Text = BatchJob.CurrentStatus.ToString();
                }

                // if job finished, hide Execute now button
                if (BatchJob.CurrentStatus != BatchStatus.Finished &&
                    BatchJob.CurrentStatus != BatchStatus.Running &&
                    BatchJob.CurrentStatus != BatchStatus.InStack)
                {
                    btnExecute.Visible = true;
                    btnSchedule.Visible = BatchJob.BatchSchedule == null;
                    linkRefresh.Visible = false;
                }
                else if (BatchJob.CurrentStatus == BatchStatus.Running || BatchJob.CurrentStatus == BatchStatus.Created ||
                    BatchJob.CurrentStatus == BatchStatus.InStack)
                {
                    linkRefresh.NavigateUrl = Request.Url.OriginalString;
                    linkRefresh.Visible = true;
                }
                string postbackEvent = Page.ClientScript.GetPostBackEventReference(btnUnSchedule, "");
                btnUnSchedule.OnClientClick = "return ShowConfirmationDialog(function(){ " + postbackEvent + " });";
            }
            else
            {
                Response.Redirect("/admin/Batch/");
            }
        }

        protected void ExecuteNow(object sender, EventArgs e)
        {
            BatchJobManager.StackForExecution(BatchJob);
            Response.Redirect(Request.Url.OriginalString);
        }

        protected void btnSchedule_Click(object sender, EventArgs e)
        {
            Response.Redirect("/admin/Batch/Edit.aspx?BatchUid=" + BatchJob.UID);
        }

        protected void btnUnSchedule_Click(object sender, EventArgs e)
        {
            BatchJob.UnSchedule();
            BatchJobManager.SaveJob(BatchJob);
        }

        protected void ButtonEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/admin/Batch/Edit.aspx?BatchUid={0}", BatchJob.UID));
        }

        protected string HumanizeRepeatHours(double hours)
        {
            string verbal = "hours";
            int intHours;
            int value = intHours = (int)hours;
            switch (intHours)
            {
                case 0:
                    value = (int)Math.Floor(hours * 60);
                    verbal = "minutes";
                    break;
                case 1:
                    verbal = "hour";
                    break;

                case 24:
                    verbal = "day";
                    value = intHours / 24;
                    break;

                case 48:
                case 72:
                    value = intHours / 24;
                    verbal = "days";
                    break;

                case 168:
                    verbal = "week";
                    value = 1;
                    break;

                case 336:
                case 504:
                case 672:
                    value = intHours / 24 / 7;
                    verbal = "weeks";
                    break;

                default:
                    break;
            }
            return string.Format("{0} {1}", value, verbal);
        }

        protected void ActionsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var action = e.Row.DataItem as BatchAction;
                if (action != null && action.IsMandatory)
                    (e.Row.Cells[e.Row.Cells.Count - 1].Controls[0]).Visible = false;
            }
        }

        protected void ActionsGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var uid = (long)e.Keys[0];
            var action = UnitOfWork.BatchActionRepository.Single(a => a.BatchActionUid == uid);
            UnitOfWork.BatchActionRepository.Delete(action);
            UnitOfWork.Commit();
            Response.Redirect(Request.Url.OriginalString);
        }
    }
}