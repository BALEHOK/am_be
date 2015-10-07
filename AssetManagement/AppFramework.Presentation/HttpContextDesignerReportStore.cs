using DevExpress.XtraReports.Service.Extensions;
using System.Web;

namespace AppFramework.Presentation
{
    public class HttpContextDesignerReportStore:  IDesignerReportStore
    {
        public void SaveLayout(string reportName, byte[] layoutData)
        {
            HttpContext.Current.Session[reportName] = layoutData;
        }

        public byte[] LoadLayout(string reportName)
        {
            return (byte[])HttpContext.Current.Session[reportName];
        }
    }
}
