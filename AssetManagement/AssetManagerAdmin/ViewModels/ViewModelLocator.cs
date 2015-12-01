/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:AssetManagerAdmin.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Microsoft.Practices.ServiceLocation;

namespace AssetManagerAdmin.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        public const string AuthViewKey = "AuthView";

        public const string LoginViewKey = "LoginView";

        public const string FormulaBuilderKey = "FormulaBuilder";

        public const string ReportsBuilderKey = "ReportsBuilder";

        public const string ValidationBuilderKey = "ValidationBuilder";

        static ViewModelLocator()
        {
            Infrastructure.Registrations.Configuration.RegisterDependencies();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public WebAdminViewModel WebAdmin
        {
            get { return ServiceLocator.Current.GetInstance<WebAdminViewModel>(); }
        }

        public FormulaBuilderViewModel FormulaBuilder
        {
            get { return ServiceLocator.Current.GetInstance<FormulaBuilderViewModel>(); }
        }

        public ValidationBuilderViewModel ValidationBuilder
        {
            get { return ServiceLocator.Current.GetInstance<ValidationBuilderViewModel>(); }
        }

        public ReportsBuilderViewModel ReportsBuilder
        {
            get { return ServiceLocator.Current.GetInstance<ReportsBuilderViewModel>(); }
        }

        public LoginViewModel Login
        {
            get { return ServiceLocator.Current.GetInstance<LoginViewModel>(); }
        }

        public AuthViewModel Auth
        {
            get { return ServiceLocator.Current.GetInstance<AuthViewModel>(); }
        }

        public RenderReportViewModel RenderReport
        {
            get { return ServiceLocator.Current.GetInstance<RenderReportViewModel>(); }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}