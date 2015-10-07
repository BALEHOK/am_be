using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Reports;
using System;

namespace AssetSite.admin.Reports.AddOnView
{
    public partial class Default : BasePage
    {
        protected void SaveReportClick(object sender, EventArgs e)
        {
            if (TemplateFile.HasFile)
            {
                Report rep = new Report(ReportName.Text, false, 0, AuthenticationService, AssetTypeRepository, AssetsService);
                string templName = Guid.NewGuid() + ".rpt";
                string fileName = Server.MapPath(ApplicationSettings.TemplatesPath) + templName;
                TemplateFile.SaveAs(fileName);
                rep.Template = templName;
                rep.Type = Report.ReportType.ViewBased;
                rep.Save();
                Response.Redirect("~/admin/Reports/Default.aspx");
            }
        }
    }
}