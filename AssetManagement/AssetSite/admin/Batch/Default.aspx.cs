using System.Data;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Batch;
using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.Batch
{
    public partial class Default : BasePage
    {
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }

        private const string SelectCommand = @"SELECT 
                bj.BatchUid,
                bj.Title, 
                bj.StartDate, 
                bj.EndDate, 
                bj.Status, 
                bs.IsEnabled, 
                u.Name AS UserName
                FROM BatchJob AS bj
                LEFT JOIN BatchSchedule AS bs ON bj.BatchScheduleId = bs.ScheduleId
                LEFT JOIN ADynEntityUser AS u ON bj.OwnerId = u.DynEntityId AND u.ActiveVersion = 1 ";

        protected void Page_Load(object sender, EventArgs e)
        {
           
            BatchDataSource.ConnectionString = UnitOfWork.SqlProvider.ConnectionString;
            BatchDataSource.SelectCommand = SelectCommand;
            InitDataSource();
        }

        protected void JobsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var job = e.Row.DataItem as DataRowView;
                var status = (Int16)job["Status"];
                if (status == (int)BatchStatus.Running || status == (int)BatchStatus.InStack)
                {
                    // hide 'Execute now' button
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Clear();
                }
                var lblStatus = e.Row.FindControl("lblStatus") as Label;
                lblStatus.Text = ((BatchStatus)status).ToString();
            }
        }

        protected void chkHideFinished_Change(object sender, EventArgs e)
        {
            InitDataSource();
            JobsGrid.PageIndex = 0;
            JobsGrid.DataBind();
        }

        public void InitDataSource()
        {
            BatchDataSource.SelectCommand =
                SelectCommand +
                (chkHideFinished.Checked
                    ? string.Format(" WHERE bj.Status != {0} ", (short) BatchStatus.Finished)
                    : string.Empty);
        }

        protected void btnCleanAll_Click(object sender, EventArgs e)
        {
            BatchJobManager.DeleteAllJobs();
            Response.Redirect("/admin/Batch/");
        }

        protected void btnCleanFinished_Click(object sender, EventArgs e)
        {
            BatchJobManager.DeleteFinishedJobs();
            Response.Redirect("/admin/Batch/");
        }
    }
}
