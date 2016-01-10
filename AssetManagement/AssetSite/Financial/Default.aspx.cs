using System;
using System.Web.UI.WebControls;

namespace AssetSite.Financial
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        protected void AddReportClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Reports/Add/");
        }

        protected void ReportsListRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Response.Redirect(Request.Url.OriginalString);
        }
    }
}
