using AppFramework.Reports.CustomReports;
using AppFramework.Reports.Services;
using Microsoft.Practices.Unity;
using AppFramework.Reports.StandardReports.DataProviders;

namespace AppFramework.Reports
{
    public class ReportsConfiguration : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterType<ICustomReportService, DevExpressCustomReportsService>()
                .RegisterType<IStandardReportService, DevExpressStandardReportService>()
                .RegisterType<IReportDataProvider<AssetXtraReport>, AssetReportDataProvider>()
                .RegisterType<IReportDataProvider<AssetsWithChildsReport>, AssetWithChildsReportDataProvider>()
                .RegisterType<IReportDataProvider<AssetsListXtraReport>, AssetsListReportDataProvider>()
                .RegisterType<IReportDataProvider<SearchResultXtraReport>, SearchResultReportDataProvider>()
                .RegisterType<IDataProviderFactory, DataProviderFactory>();
        }
    }
}
