using AssetManagerAdmin.Model;
using AssetManagerAdmin.WebApi;
using GalaSoft.MvvmLight.Ioc;
using AssetManagerAdmin.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Common.Logging;

namespace AssetManagerAdmin
{
    public static class Registrations
    {
        public static void RegisterDependencies()
        {
            ServiceLocator.SetLocatorProvider(
                () => SimpleIoc.Default);
            SimpleIoc.Default.Register(() => LogManager.GetLogger("AssetManagerAdmin"));
            SimpleIoc.Default.Register<IAssetsApiManager, AssetsApiManager>();
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<WebAdminViewModel>();
            SimpleIoc.Default.Register<FormulaBuilderViewModel>();
            SimpleIoc.Default.Register<ValidationBuilderViewModel>();
            SimpleIoc.Default.Register<ReportsBuilderViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<AuthViewModel>();
        }
    }
}
