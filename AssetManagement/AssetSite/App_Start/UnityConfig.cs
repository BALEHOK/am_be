using AppFramework.Core;
using AppFramework.Core.AC.Providers;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Reports;
using AssetManager.Infrastructure;

namespace AssetSite
{
    public static class UnityConfig
    {
        public static void RegisterComponents(IUnityContainer container)
        {
            container.RegisterType<IUnitOfWork, UnitOfWork>(
                    new PerRequestLifetimeManager(),
                    new InjectionFactory(c => new UnitOfWork()))
                .RegisterType<IAuthenticationStorageProvider,
                    InMemoryAuthenticationStorageProvider>()
                .RegisterType<ITypeSearch, TypeSearch>()
                .AddNewExtension<InfrastructureConfiguration>()
                .AddNewExtension<CommonConfiguration>()
                .AddNewExtension<ReportsConfiguration>();
        }
    }
}