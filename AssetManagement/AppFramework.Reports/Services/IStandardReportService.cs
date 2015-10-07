using DevExpress.XtraReports.UI;
using System;

namespace AppFramework.Reports.Services
{
    public interface IStandardReportService
    {
        XtraReport GetStandardReport(ReportType reportType, ReportLayout layout = ReportLayout.Default);
    }
}
