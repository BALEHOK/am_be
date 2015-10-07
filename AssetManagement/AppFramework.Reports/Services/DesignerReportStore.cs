using DevExpress.XtraReports.Service.Extensions;
using System.ComponentModel.Composition;
using System.IO;

namespace AppFramework.Reports.Services
{
    [Export(typeof(IDesignerReportStore))]
    public class DesignerReportStore : IDesignerReportStore
    {
        byte[] IDesignerReportStore.LoadLayout(string reportName)
        {
            return File.ReadAllBytes(@"C:\temp\layout.repx");
        }

        void IDesignerReportStore.SaveLayout(string reportName, byte[] layoutData)
        {
            File.WriteAllBytes(@"C:\temp\layout.repx", layoutData);
        }
    }
}
