using AppFramework.Reports.Permissions;
using AppFramework.Reports.Services;
using AppFramework.Reports.StandardReports.DataProviders;
using Microsoft.Practices.Unity;

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
                .RegisterType<IDataProviderFactory, DataProviderFactory>()
                .RegisterType<IReportPermissionChecker, ReportPermissionChecker>();
        }
    }
}