using DevExpress.XtraReports.UI;

namespace AppFramework.Reports.StandardReports.DataProviders
{
    public interface IDataProviderFactory
    {
        IReportDataProvider<T> GetDataProvider<T>() where T : XtraReport;
    }
}