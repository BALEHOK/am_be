using DevExpress.XtraReports.UI;

namespace AppFramework.Reports.Services
{
    public class DevExpressStandardReportService : IStandardReportService
    {
        public XtraReport GetStandardReport(ReportType reportType, LayoutType layout = LayoutType.Default)
        {
            var report = ReportFactory.BuildReport(reportType, layout);
            return report;
        }
    }
}
