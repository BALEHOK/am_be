using System;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;


namespace AssetManagementBatch
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            this.Installers.Add(GetServiceInstaller());
            this.Installers.Add(GetServiceProcessInstaller());
        }

        private ServiceInstaller GetServiceInstaller()
        {
            var installer = new ServiceInstaller
            {
                ServiceName = GetConfigurationValue("ServiceName"),
                StartType = ServiceStartMode.Automatic
            };
            return installer;
        }

        private ServiceProcessInstaller GetServiceProcessInstaller()
        {
            var installer = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem,
            };
            return installer;
        }

        private string GetConfigurationValue(string key)
        {
            Assembly service = Assembly.GetAssembly(typeof(BatchService));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] != null)
            {
                return config.AppSettings.Settings[key].Value;
            }
            throw new IndexOutOfRangeException("Settings collection does not contain the requested key:" + key);
        }
    }
}
