using AssetManagerAdmin.Model;
using AssetManagerAdmin.WebApi;
using Microsoft.Practices.ServiceLocation;
using Common.Logging;
using Microsoft.Practices.Unity;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity.InterceptionExtension;
using AssetManagerAdmin.FormulaBuilder.Expressions;

namespace AssetManagerAdmin.Infrastructure.Registrations
{
    public static class Configuration
    {
        public static void RegisterDependencies()
        {
            var container = new UnityContainer();
            ServiceLocator.SetLocatorProvider(
                () => new UnityServiceLocator(container));

            container
                .AddNewExtension<Interception>()
                .AddNewExtension<NavigationRegistration>()
                .RegisterInstance<IAppContext>(new AppContext())
                .RegisterType<ILog>(new InjectionFactory((x, t, s) => LogManager.GetLogger(t)))
                .RegisterType<IMessenger>(new InjectionFactory((x, t, s) => Messenger.Default))
                .RegisterType<IAssetsApiManager, AssetsApiManager>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IDataService, DataService>()
                .RegisterType<IAssetsApi, AssetsApiCacheDecorator>(
                    new PerResolveLifetimeManager(),
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<ProgressBarInterceptionBehavior>())
                .RegisterType<IDialogService, DialogService>()
                .RegisterType<IAssetsDataProvider, AssetsDataProvider>();
        }
    }
}
