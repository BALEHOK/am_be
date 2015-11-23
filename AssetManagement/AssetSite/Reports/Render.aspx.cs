using AppFramework.Reports;
using System;
using AppFramework.Reports.CustomReports;
using AppFramework.Reports.Services;
using Microsoft.Practices.Unity;

namespace AssetSite.Reports
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Render : BasePage
    {
        /// <summary>
        /// 
        /// </summary>
        [Dependency]
        public ICustomReportService<CustomDevExpressReport> ReportService { get; set; }

        protected IReport Report { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["LayoutId"] != null)
            {
                int layoutId;
                int.TryParse(Request["LayoutId"], out layoutId);
                var layout = UnitOfWork.ReportLayoutRepository.SingleOrDefault(l => l.Id == layoutId);
                this.Report = ReportFactory.BuildReport(layout) as IReport;
            }
            else if (Request["ReportType"] != null)
            {
                int reportType;
                int.TryParse(Request["ReportType"], out reportType);
                int reportLayout = 0;
                int.TryParse(Request["ReportLayout"], out reportLayout);
                this.Report = ReportFactory.BuildReport(
                    (ReportType)reportType,
                    (AppFramework.Reports.ReportLayout)reportLayout) as IReport;
            }
            else
            {
                throw new ArgumentException("LayoutId or ReportType parameters should be provided.");
            }

            if (!IsPostBack)
            {
                if (Report.ReportType == ReportType.AssetsListReport)
                {
                    var assetTypes = AssetTypeRepository.GetAllPublished();
                    lbAssetTypes.DataSource = assetTypes;
                    lbAssetTypes.DataBind();
                }
            }
            
            Guid searchId; 
            if (!Guid.TryParse(Request["SearchId"], out searchId))
            {
                searchId = Guid.NewGuid();
            }
            var reportId = long.Parse(Request["ReportId"]);

            var report = ReportService.GetReportById(reportId);
            
            var filter = new SearchResultReportFilter(UnitOfWork);
            var filterString = filter.GetFilterString(searchId);
            report.Filter = filterString;
            
            ReportViewer.Report = report.ReportObject;
            ReportViewer.Visible = true;

            Unload += (o, args) => { report.ReportObject.Dispose(); };
        }

        protected void OnAssetTypeSelected(object sender, EventArgs e)
        {
            var selectedItem = lbAssetTypes.SelectedItem;
            if (selectedItem == null)
                return;
            Response.Redirect(string.Format("~/Reports/Render.aspx?AssetTypeId={0}&ReportType={1}",
                selectedItem.Value, (int)ReportType.AssetsListReport));
        }
    }
}