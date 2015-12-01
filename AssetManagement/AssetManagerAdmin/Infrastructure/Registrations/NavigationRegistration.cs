using AssetManagerAdmin.Services;
using AssetManagerAdmin.ViewModels;
using Microsoft.Practices.Unity;
using System;

namespace AssetManagerAdmin.Infrastructure
{
    class NavigationRegistration : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure(ViewModelLocator.AuthViewKey,
                new Uri("Views/AuthView.xaml", UriKind.Relative));
            navigationService.Configure(ViewModelLocator.LoginViewKey,
                new Uri("Views/LoginView.xaml", UriKind.Relative));
            navigationService.Configure(ViewModelLocator.FormulaBuilderKey,
                new Uri("Views/FormulaBuilderView.xaml", UriKind.Relative));
            navigationService.Configure(ViewModelLocator.ReportsBuilderKey,
                new Uri("Views/ReportsBuilderView.xaml", UriKind.Relative));
            navigationService.Configure(ViewModelLocator.ValidationBuilderKey,
                new Uri("Views/ValidationBuilderView.xaml", UriKind.Relative));

            Container.RegisterInstance<IFrameNavigationService>(navigationService);

            Container.RegisterType<MainViewModel>()
                .RegisterType<WebAdminViewModel>()
                .RegisterType<FormulaBuilderViewModel>()
                .RegisterType<ValidationBuilderViewModel>()
                .RegisterType<ReportsBuilderViewModel>()
                .RegisterType<LoginViewModel>()
                .RegisterType<AuthViewModel>();
        }
    }
}
