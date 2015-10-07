using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AssetUpdater.Code;
using AssetUpdater.Code.Arguments;
using AssetUpdater.AssetManagementInformationService;
using System.Windows.Media.Animation;

namespace AssetUpdater.Controls
{
    /// <summary>
    /// Interaction logic for VersionCheckControl.xaml
    /// </summary>
    public partial class SplashControl : UserControl
    {
        public event SplashActionDoneDelegate OnGotVersion, OnGotScripts, OnGotPackage;

        InformationServiceSoapClient svc = new InformationServiceSoapClient();

        public SplashControl()
        {
            InitializeComponent();
            svc.GetVersionCompleted += new EventHandler<GetVersionCompletedEventArgs>(svc_GetVersionCompleted);
            svc.GetScriptsCompleted += new EventHandler<GetScriptsCompletedEventArgs>(svc_GetScriptsCompleted);
            svc.GetPackageCompleted += new EventHandler<GetPackageCompletedEventArgs>(svc_GetPackageCompleted);
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Duration duration = new Duration(TimeSpan.FromSeconds(1));
            DoubleAnimation doubleanimation = new DoubleAnimation(100.0, duration);
            doubleanimation.RepeatBehavior = RepeatBehavior.Forever;
            pbAction.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
        }

        public void ExecuteGetVersion()
        {
            tblkMessage.Text = "Retriving Latest Version";
            svc.GetVersionAsync();
        }
        public void ExecuteGetScriptsLocation(string version)
        {
            tblkMessage.Text = "Retriving SQL scripts location";
            svc.GetScriptsAsync(version);
        }
        public void ExecuteGetPackageLocation(string version)
        {
            tblkMessage.Text = "Retriving Package location";
            svc.GetPackageAsync(version);
        }

        void svc_GetScriptsCompleted(object sender, GetScriptsCompletedEventArgs e)
        {
            if (OnGotScripts != null)
                OnGotScripts(new object(), new SplashActionDoneEventArgs() { Info = e.Result });
        }

        void svc_GetVersionCompleted(object sender, GetVersionCompletedEventArgs e)
        {
            if (OnGotVersion != null)
                OnGotVersion(new object(), new SplashActionDoneEventArgs() { Info = e.Result });
        }

        void svc_GetPackageCompleted(object sender, GetPackageCompletedEventArgs e)
        {
            if (OnGotPackage != null)
                OnGotPackage(new object(), new SplashActionDoneEventArgs() { Info = e.Result });
        }
    }
}
