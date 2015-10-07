using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace AssetUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var allProce = Process.GetProcesses();
            var process = Process.GetProcesses().Where(p => p.ProcessName.ToLower() == "assetupdater");
            if (process.Count() > 1)
            {
                MessageBox.Show("Only one instance of Asset Manager Updater allowed. Terminating...");
                App.Current.Shutdown();
            }

            MainWindow wnd = new MainWindow();
            wnd.Show();
        }
    }
}
