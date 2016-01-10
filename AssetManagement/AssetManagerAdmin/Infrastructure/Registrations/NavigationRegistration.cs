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
            navigationService.Configure(ViewModelLocator.ReportBuilderKey,
                new Uri("Views/ReportBuilderView.xaml", UriKind.Relative));
            navigationService.Configure(ViewModelLocator.ValidationBuilderKey,
                new Uri("Views/ValidationBuilderView.xaml", UriKind.Relative));

            Container.RegisterType<MainViewModel>()
                .RegisterType<WebAdminViewModel>()
                .RegisterType<FormulaBuilderViewModel>()
                .RegisterType<ValidationBuilderViewModel>()
                .RegisterType<OpenReportDialogViewModel>()
                .RegisterType<ReportBuilderViewModel>()
                .RegisterType<LoginViewModel>()
                .RegisterType<AuthViewModel>();

            Container.RegisterInstance<IFrameNavigationService>(navigationService);
        }
    }
}
