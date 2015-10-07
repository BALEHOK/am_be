using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes.Reports;

namespace AssetSite.Financial
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportsList.DataSource = Report.GetAll().Where(r => r.IsFinancial);
                ReportsList.DataBind();
            }
        }

        protected void AddReportClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Reports/Add/");
        }

        protected void ReportsListRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //ReportsList.DataKeys[e.Row.RowIndex].Value
            long uid = 0;
            GridViewRow row = ReportsList.Rows[e.RowIndex];
            long.TryParse((row.Cells[0].FindControl("UID") as HiddenField).Value, out uid);

            if (uid != 0)
            {
                Report.GetByUid(uid).Delete();
            }

            Response.Redirect(Request.Url.OriginalString);
        }
    }
}
