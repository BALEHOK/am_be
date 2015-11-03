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
                .RegisterType<IAssetTypeService, AssetTypeService>()
                .RegisterType<IDataTypeService, DataTypeService>()
                .RegisterType<IExportService, ExportService>()
                .RegisterType<IFileService, FileService>()
                .RegisterType<IEnvironmentSettings, EnvironmentSettings>()
                .RegisterType<ITaxonomyService, TaxonomyService>()
                .RegisterType<IDocumentService, DocumentService>()
                .RegisterType<IModelFactory, ModelFactory>()
                .RegisterType<IHistoryService, HistoryService>()
                .RegisterType<ITasksService, TasksService>();
        }
    }
}
