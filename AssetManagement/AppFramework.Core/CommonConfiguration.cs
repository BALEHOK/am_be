using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.Batch;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.IE.Adapters;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.Tasks.Runners;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.Interceptors;
using AppFramework.Core.PL;
using AppFramework.Core.Services;
using AppFramework.Core.Validation;
using Common.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace AppFramework.Core
{
    public class CommonConfiguration : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterType<ILog>(new InjectionFactory((x, t, s) => LogManager.GetLogger(t)))
                .RegisterType<IAssetTypeRepository, AssetTypeRepository>(
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<TransactionInterceptor>())
                .RegisterType<IAttributeRepository, AttributeRepository>()
                .RegisterType<IAssetsService, AssetsService>()
                .RegisterType<IDataTypeService, DataTypeService>()
                .RegisterType<IAuthenticationService, AuthenticationService>()
                .RegisterType<ILinkedEntityFinder, LinkedEntityFinder>()
                .RegisterType<IUserService, UserService>()
                .RegisterType<ILayoutRepository, LayoutRepository>()
                .RegisterType<ITaxonomyService, TaxonomyService>()
                .RegisterType<ITaxonomyItemService, TaxonomyItemService>()
                .RegisterType<IBatchJobManager, BatchJobManager>()
                .RegisterType<IBatchActionFactory, BatchActionFactory>()
                .RegisterType<IBatchJobFactory, BatchJobFactory>()
                .RegisterType<IAttributeFieldFactory, AttributeFieldFactory>()
                .RegisterType<IBarcodeProvider, DefaultBarcodeProvider>()
                .RegisterType<IValidationOperatorFactory, ValidationOperatorFactory>()
                .RegisterType<IValidationRulesService, ValidationRulesService>()
                .RegisterType<IValidationService, ValidationService>()
                .RegisterType<IValidationServiceNew, ValidationServiceNew>()
                .RegisterType<IXMLToAssetsAdapter, XMLToAssetsAdapter>()
                .RegisterType<IAssetTypeTaxonomyManager, AssetTypeTaxonomyManager>()
                .RegisterType<IAttributeCalculator, AttributeCalculator>()
                .RegisterType<IAssetTemplateService, AssetTemplateService>()
                .RegisterType<IDynListItemService, DynListItemService>()
                .RegisterType<IDynamicListsService, DynamicListsService>()
                .RegisterType<IPanelsService, PanelsService>()
                .RegisterType<ITableProvider, DynTableProvider>()
                .RegisterType<IDynColumnAdapter, DynColumnAdapter>()
                .RegisterType<IScreensService, ScreensService>()
                .RegisterType<ISearchService, SearchEngine>()
                .RegisterType<IDataFactory, DataFactory>()
                .RegisterType<IAssetPanelsAdapter, AssetPanelsAdapter>()
                .RegisterType<ISearchTracker, SearchTracker>()
                .RegisterType<ITaskRunnerFactory, TaskRunnerFactory>()
                .RegisterType<IAttributeValueFormatter, AttributeValueFormatter>()
                .RegisterType<IImportExportManager, ImportExportManager>()
                .RegisterType<IRightsService, RightsService>()
                .RegisterType<IRoleService, RoleService>();
        }
    }
}