namespace AssetSite.admin.Reports.Add
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using AppFramework.Core.Classes.Reports;

    public partial class AssetInfo : ReportPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("Content-Type", "text/plain");            
            Response.AddHeader("Content-Disposition", "attachment; filename=AssetInfo.ttx");
            // Response.AddHeader("filename", "AssetInfo.ttx");

            if (String.IsNullOrEmpty(Request.QueryString["fi"]))
            {
                foreach (ReportField field in this.report.Fields.Where(f => f.IsVisible))
                {
                    Response.Write(field.GetTtxLine());
                }
            }
            else
            {
                
            }
            Response.Write("AssetType\tString\t200" + System.Environment.NewLine);
            Response.Write("AssetTypeId\tString\t200" + System.Environment.NewLine);
            Response.Write("AssetId\tString\t200");
        }
    }
}
