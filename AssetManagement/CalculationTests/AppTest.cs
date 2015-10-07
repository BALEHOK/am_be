using AppFramework.Core;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.AC.Providers;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.Interfaces;
using AppFramework.DataProxy;
using AssetSite;
using Microsoft.Practices.Unity;

namespace CalculationTests
{
    public class AppTest
    {
        public IUnityContainer Container;
        public IUnitOfWork UnitOfWork;
        public IDataTypeService DataTypeService;
        public IDynColumnAdapter DynColumnAdapter;
        public IAssetTypeRepository AssetTypeRepository;
        public IAssetsService AssetsService;
        public IScreensService ScreensService;
        public ILayoutRepository LayoutRepository;
        public DynTableProvider DynTableProvider;
        public ILinkedEntityFinder LinkedEntityFinder;
        public IAuthenticationService AuthenticationService;
        public IAttributeCalculator AttributeCalculator;

        protected Helper Helper;

        protected void InitUnity()
        {
            AutoMapperConfig.RegisterDataMappings();
            var application = (IHttpUnityApplication) new Global();
            Container = application.UnityContainer;
            Container.RegisterType<IUnitOfWork, UnitOfWork>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => new UnitOfWork()))
                .RegisterType<IAuthenticationStorageProvider,
                    InMemoryAuthenticationStorageProvider>()
                .AddNewExtension<CommonConfiguration>();

            UnitOfWork = Container.Resolve<IUnitOfWork>();
            DataTypeService = Container.Resolve<IDataTypeService>();
            DynColumnAdapter = Container.Resolve<DynColumnAdapter>();
            AssetTypeRepository = Container.Resolve<IAssetTypeRepository>();
            AssetsService = Container.Resolve<IAssetsService>();
            ScreensService = Container.Resolve<IScreensService>();
            LayoutRepository = Container.Resolve<ILayoutRepository>();
            DynTableProvider = Container.Resolve<DynTableProvider>();
            LinkedEntityFinder = Container.Resolve<ILinkedEntityFinder>();
            AuthenticationService = Container.Resolve<IAuthenticationService>();
            AttributeCalculator = Container.Resolve<AttributeCalculator>();

            Helper = new Helper(this);
        }
    }
}