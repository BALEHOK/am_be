using System;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Common.Logging;

namespace AssetManagerAdmin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void CallUi(Action action)
        {
            Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        static App()
        {
            DispatcherHelper.Initialize();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.GetLogger<App>().Error(e.ExceptionObject);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            base.OnStartup(e);
        }
    }
}
