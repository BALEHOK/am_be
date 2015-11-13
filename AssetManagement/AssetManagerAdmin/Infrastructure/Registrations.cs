using AssetManagerAdmin.Model;
using AssetManagerAdmin.WebApi;
using AssetManagerAdmin.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Common.Logging;
using Microsoft.Practices.Unity;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace AssetManagerAdmin
{
    public static class Registrations
    {
        public static void RegisterDependencies()
        {
            var container = new UnityContainer();
            ServiceLocator.SetLocatorProvider(
                () => new UnityServiceLocator(container));

            container.AddNewExtension<Interception>();
            container.RegisterType<ILog>(new InjectionFactory((x, t, s) => LogManager.GetLogger(t)));
            container.RegisterType<IMessenger>(new InjectionFactory((x, t, s) => Messenger.Default));
            container.RegisterType<IAssetsApiManager, AssetsApiManager>();
            container.RegisterType<IDataService, DataService>(
              new Interceptor<InterfaceInterceptor>(),
              new InterceptionBehavior<ProgressBarInterceptionBehavior>());
            container.RegisterType<IDialogService, DialogService>();
            container.RegisterType<MainViewModel>();
            container.RegisterType<WebAdminViewModel>();
            container.RegisterType<FormulaBuilderViewModel>();
            container.RegisterType<ValidationBuilderViewModel>();
            container.RegisterType<ReportsBuilderViewModel>();
            container.RegisterType<LoginViewModel>();
            container.RegisterType<AuthViewModel>();
        }
    }
}
