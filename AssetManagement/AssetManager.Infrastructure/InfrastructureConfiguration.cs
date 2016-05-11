using System.Web.Security;
using AppFramework.Core.AC.Providers;
using AssetManager.Infrastructure.Permissions;
using AssetManager.Infrastructure.Services;
using Microsoft.Practices.Unity;

namespace AssetManager.Infrastructure
{
    public class InfrastructureConfiguration : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterType<IBarcodeService, BarcodeService>()
                .RegisterType<IAssetService, AssetService>()
                .RegisterType<IPasswordEncoder>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionFactory(c => (IPasswordEncoder)Membership.Provider)
                )
                .RegisterType<IAssetTypeService, AssetTypeService>()
                .RegisterType<IAssetPermissionChecker, AssetPermissionChecker>()
                .RegisterType<IAssetTypePermissionChecker, AssetTypePermissionChecker>()
                .RegisterType<IDataTypeService, DataTypeService>()
                .RegisterType<IExportService, ExportProxy>()
                .RegisterType<IFileService, FileService>()
                .RegisterType<IEnvironmentSettings, EnvironmentSettings>()
                .RegisterType<ITaxonomyService, TaxonomyService>()
                .RegisterType<IDocumentService, DocumentService>()
                .RegisterType<IModelFactory, ModelFactory>()
                .RegisterType<IHistoryService, HistoryService>()
                .RegisterType<ITasksService, TasksService>()
                .RegisterType<IFaqService, FaqService>()
                .RegisterType<IPermissionsService, PermissionsService>()
                .RegisterType<IBannerService, BannerService>();
        }
    }
}
