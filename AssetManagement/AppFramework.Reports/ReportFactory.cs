using DevExpress.XtraReports.UI;
using System;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;

namespace AppFramework.Reports
{
    public class ReportFactory
    {
        public static XtraReport BuildReport(ReportType reportType, LayoutType layout = LayoutType.Default)
        {
            var @interface = typeof(IReport);
            var report = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsSubclassOf(typeof(XtraReport)) && p.GetInterfaces().Contains(@interface))
                .Select(r => Activator.CreateInstance(r) as IReport)
                .FirstOrDefault(r => r.ReportType == reportType && r.ReportLayout == layout);

            if (report == null)
                throw new InstanceNotFoundException(string.Format("Cannot create XtraReport instance of type: {0}",
                    reportType));
            return report as XtraReport;
        }
    }
}
