using DevExpress.XtraReports.Service;
using DevExpress.XtraReports.Service.Extensions;
using System.ComponentModel.Composition;

namespace AppFramework.Reports.Services
{
    [Export(typeof(IDataSourceService))]
    public class DataSourceService : IDataSourceService
    {
        private const string DataSourceName = "TestDataSource";

        public void FillDataSources(DevExpress.XtraReports.UI.XtraReport report, string reportName, bool isDesignSessionActive)
        {
            var dataSet = report.GetDataSourceByName(DataSourceName);
        }

        public void RegisterDataSources(DevExpress.XtraReports.UI.XtraReport report, string reportName)
        {
            if (report.DataSource != null)
                report.RegisterDataSourceName(DataSourceName, report.DataSource);
        }
    }
}
