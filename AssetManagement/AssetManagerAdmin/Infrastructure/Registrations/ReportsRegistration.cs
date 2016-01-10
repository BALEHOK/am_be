using AppFramework.DataProxy;
using AppFramework.Reports.CustomReports;
using AppFramework.Reports.Services;
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
            Container
                .RegisterType<IUnitOfWork, UnitOfWork>(
                    new TransientLifetimeManager(),
                    new InjectionFactory(c => new UnitOfWork()))
                .RegisterType<IStandardReportService, DevExpressStandardReportService>()
                .RegisterType<ICustomReportService, DevExpressCustomReportsService>()
                .RegisterType<IReportStorage, DbReportStorage>()
                .RegisterType<IReportResolver, ReportResolver>();
        }
    }
}
