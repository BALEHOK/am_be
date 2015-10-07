using AppFramework.Core;
using AppFramework.Core.AC.Providers;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity;
using System.Web;
using AssetManager.Infrastructure.Services;
using AssetManager.Infrastructure;
using AppFramework.Reports;

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
                .RegisterType<IBarcodeService, BarcodeService>()
                .RegisterType<IAssetService, AssetService>()
                .RegisterType<IAssetTypeService, AssetTypeService>()
                .RegisterType<IExportService, ExportService>()
                .RegisterType<IFileService, FileService>()
                .RegisterType<IEnvironmentSettings, EnvironmentSettings>()
                .RegisterType<ITaxonomyService, TaxonomyService>()
                .RegisterType<IDocumentService, DocumentService>()
                .RegisterType<IModelFactory, ModelFactory>()
                .RegisterType<ITasksService, TasksService>()                
                .AddNewExtension<CommonConfiguration>()
                .AddNewExtension<ReportsConfiguration>();
        }
    }
}