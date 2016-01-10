using DevExpress.XtraReports.UI;
using System;

namespace AppFramework.Reports.Services
{
    public interface IStandardReportService
    {
        XtraReport GetStandardReport(ReportType reportType, LayoutType layout = LayoutType.Default);
    }
}
