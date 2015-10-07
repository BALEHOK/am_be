using AppFramework.DataProxy;
using System;

namespace AssetSite.Reports
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var layoutsRepository = UnitOfWork.ReportLayoutRepository;
            var layouts = layoutsRepository.Get();
            gvReports.DataSource = layouts;
            gvReports.DataBind();
        }
    }
}