using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Reports.Services
{
    public class DevExpressStandardReportService : IStandardReportService
    {
        public DevExpressStandardReportService()
        {

        }

        public XtraReport GetStandardReport(ReportType reportType, ReportLayout layout = ReportLayout.Default)
        {
            var report = ReportFactory.BuildReport(reportType, layout);
            return report;
        }
    }
}
