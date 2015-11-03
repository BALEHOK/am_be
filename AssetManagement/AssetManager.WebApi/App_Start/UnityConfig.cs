using System;
using Microsoft.Practices.Unity;
using AppFramework.DataProxy;
using AppFramework.Core.AC.Providers;
using AppFramework.Core;
using System.Web;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Reports;
using AssetManager.WebApi.Models.Search;
using AppFramework.Email;
using AssetManager.Infrastructure;

namespace AssetManager.WebApi
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container
                .RegisterType<IUnitOfWork, UnitOfWork>(
                    new PerRequestLifetimeManager(),
                    new InjectionFactory(c => new UnitOfWork()))
                // TODO: next 3 registrations could be moved to CommonConfiguration (?)
                .RegisterType<IAuthenticationStorageProvider, InMemoryAuthenticationStorageProvider>()
                .RegisterType<ISearchService, SearchEngine>()
                .RegisterType<ITypeSearch, TypeSearch>()
                .RegisterType<IAdvanceSearchModelMapper, AdvanceSearchModelMapper>()
                .AddNewExtension<InfrastructureConfiguration>()
                .AddNewExtension<EmailConfiguration>()
                .AddNewExtension<CommonConfiguration>()
                .AddNewExtension<ReportsConfiguration>()
                .RegisterType<IHttpHandler, FileHandler>("/FileHandler.ashx");
        }
    }
}
