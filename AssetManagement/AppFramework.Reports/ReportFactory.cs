using AppFramework.Entities;
using DevExpress.XtraReports.UI;
using System;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;

namespace AppFramework.Reports
{
    public class ReportFactory
    {
        public static XtraReport BuildReport(Entities.ReportLayout layout)
        {
            if (layout == null)
                throw new ArgumentNullException();

            // create report instance
            var reportTypeName = string.Format("AppFramework.Reports.{0}",
                layout.ReportTypeName.Split(new char[] { '.' }).Last());
            Type reportType = Type.GetType(reportTypeName);
            if (reportType == null)
                throw new InvalidDataException("Cannot create XtraReport instance of type: " + reportTypeName);
            var reportInstance = Activator.CreateInstance(reportType) as XtraReport;
            if (reportInstance == null)
                throw new InstanceNotFoundException("Cannot create XtraReport instance of type: " + reportTypeName);

            // load layout
            if (!string.IsNullOrEmpty(layout.FilePath))
                reportInstance.LoadLayout(layout.FilePath);

            return reportInstance;
        }

        public static XtraReport BuildReport(ReportType reportType, ReportLayout layout = ReportLayout.Default)
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
