using AppFramework.Core.Classes.ScreensServices;
using AppFramework.DataProxy;
using AppFramework.Reports.Permissions;
using AppFramework.Reports.Services;
using AssetManager.Infrastructure.Services;
using AssetManagerAdmin.Services;
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.XtraReports.Service.Extensions;
using Microsoft.Practices.Unity;

namespace AssetManagerAdmin.Infrastructure.Registrations
{
    class ReportsRegistration : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // these interfaces should be referenced only within Reports module
            // which is allowed to directly connect to database
            Container
                .RegisterType<IUnitOfWork, UnitOfWork>(
                    new TransientLifetimeManager(),
                    new InjectionFactory(c => new UnitOfWork()))
                .RegisterType<IAssetTypeService, AssetTypeService>()
                .RegisterType<IScreensService, ScreensService>() // required for AssetTypeService

                .RegisterType<IStandardReportService, DevExpressStandardReportService>()
                .RegisterType<ICustomReportService, DevExpressCustomReportsService>()
                .RegisterType<IReportStorage, DbReportStorage>()
                .RegisterType<IReportResolver, ReportResolver>()
                .RegisterType<IReportPermissionChecker, ReportPermissionChecker>();
        }
    }
}
