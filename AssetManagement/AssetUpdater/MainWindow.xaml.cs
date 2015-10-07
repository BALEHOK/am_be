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
using AssetUpdater.AssetManagementInformationService;
using System.Net;
using System.IO;
using AssetUpdater.Controls;
using AssetUpdater.Code;
using System.ComponentModel;
using System.Windows.Threading;
using Forms = System.Windows.Forms;
using System.Windows.Media.Animation;

namespace AssetUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XmlDataBase _db;
        private SplashControl splash;
        private PackageProcessor _pProcessor;

        private string AssetMgrVersion { get; set; }
        private string WebsitePath { get; set; }

        private BackgroundWorker _scriptDownloader;
        private BackgroundWorker _packageDownloader;

        public MainWindow()
        {
            InitializeComponent();

            _db = new XmlDataBase("settings.xml");

            _pProcessor = new PackageProcessor();
            _pProcessor.OnActionBegin += new PackegProcessorProgressDelegate(_pProcessor_OnActionBegin);
            _pProcessor.OnProgress += new PackegProcessorProgressDelegate(_pProcessor_OnProgress);
            _pProcessor.OnActionEnd += new PackegProcessorProgressDelegate(_pProcessor_OnActionEnded);

            _scriptDownloader = new BackgroundWorker();
            _scriptDownloader.DoWork += new DoWorkEventHandler(_scriptDownloader_DoWork);

            _packageDownloader = new BackgroundWorker();
            _packageDownloader.DoWork += new DoWorkEventHandler(_packageDownloader_DoWork);
            _packageDownloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_packageDownloader_RunWorkerCompleted);

            splash = new SplashControl();
            splash.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            splash.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            Grid.SetColumn(splash, 1);
            Grid.SetRow(splash, 1);

            splash.OnGotScripts += new Code.SplashActionDoneDelegate(splash_OnGotScripts);
            splash.OnGotVersion += new Code.SplashActionDoneDelegate(splash_OnGotVersion);
            splash.OnGotPackage += new Code.SplashActionDoneDelegate(splash_OnGotPackage);
        }

        void _packageDownloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.AddListItem("Retriving SQL Script location...", "getscripturl");

            MainContainer.Children.Add(splash);
            splash.ExecuteGetScriptsLocation(this.AssetMgrVersion);
        }

        #region Packeg Processor events
        void _pProcessor_OnProgress(object sender, Code.Arguments.PackageProcessorProgressEventArgs e)
        {
            Dispatcher.Invoke((Action<long>)(delegate(long progr)
            {
                progressBar1.Value = progr;
            }), e.Progress);
        }

        void _pProcessor_OnActionEnded(object sender, Code.Arguments.PackageProcessorProgressEventArgs e)
        {
            Dispatcher.Invoke((Action<string>)(delegate(string aid)
            {
                this.MarkAsDone(aid);
            }), e.ActionID);
        }

        void _pProcessor_OnActionBegin(object sender, Code.Arguments.PackageProcessorProgressEventArgs e)
        {
            Dispatcher.Invoke((Action<string, string>)(delegate(string info, string id)
            {
                if (e.Progress < 0)
                {
                    Duration duration = new Duration(TimeSpan.FromSeconds(1));
                    DoubleAnimation doubleanimation = new DoubleAnimation(100.0, duration);
                    doubleanimation.RepeatBehavior = RepeatBehavior.Forever;
                    progressBar1.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
                }
                else
                {
                    progressBar1.BeginAnimation(ProgressBar.ValueProperty, null);
                }

                progressBar1.Maximum = 100;
                progressBar1.Value = 0;

                this.AddListItem(info, id);
            }), e.Info, e.ActionID);
        }
        #endregion

        #region Window Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WebsitePath = _db.WebsitePath;
            if (String.IsNullOrEmpty(this.WebsitePath))
            {
                Forms.FolderBrowserDialog dlg = new Forms.FolderBrowserDialog();
                if (dlg.ShowDialog() == Forms.DialogResult.OK)
                {
                    this.WebsitePath = dlg.SelectedPath;
                }
            }

            this.AddListItem("Retriving Latest Version...", "getversion");

            MainContainer.Children.Add(splash);

            splash.ExecuteGetVersion();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _db.Version = this.AssetMgrVersion;
            _db.WebsitePath = this.WebsitePath;
            _db.Dispose();
        }

        private void btnRunUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.AddListItem("Retriving Package location...", "getpakurl");

            MainContainer.Children.Add(splash);
            splash.ExecuteGetPackageLocation(this.AssetMgrVersion);

            btnRunUpdate.IsEnabled = false;
        }
        #endregion

        #region Splash Screen Events
        void splash_OnGotVersion(object sender, Code.Arguments.SplashActionDoneEventArgs e)
        {
            MainContainer.Children.Remove(splash);

            this.AssetMgrVersion = e.Info.ToString();

            if (this.AssetMgrVersion == _db.Version)
            {
                MessageBox.Show("You have latest version, no update needed");
            }
            else
            {
                btnRunUpdate.Visibility = System.Windows.Visibility.Visible;
            }

            this.MarkAsDone("getversion");
        }

        void splash_OnGotScripts(object sender, Code.Arguments.SplashActionDoneEventArgs e)
        {
            MainContainer.Children.Remove(splash);

            FileDescriptor desc = (FileDescriptor)e.Info;

            if (desc != null)
            {
                this.MarkAsDone("getscripturl");
                this.AddListItem("Downloading SQL scripts...", "downloadscript");

                BGWorkerArgument arg = new BGWorkerArgument(desc);
                arg.IsSQL = true;

                _scriptDownloader.RunWorkerAsync(arg);
            }
            else
            {
                this.MarkAsBroken("getscripturl");
                btnRunUpdate.IsEnabled = true;
            }
        }

        void splash_OnGotPackage(object sender, Code.Arguments.SplashActionDoneEventArgs e)
        {
            MainContainer.Children.Remove(splash);
            
            FileDescriptor desc = (FileDescriptor)e.Info;

            if (desc != null)
            {
                this.MarkAsDone("getpakurl");
                this.AddListItem("Downloading package...", "downloadpak");

                BGWorkerArgument arg = new BGWorkerArgument(desc);

                _packageDownloader.RunWorkerAsync(arg);
            }
            else
            {
                this.MarkAsBroken("getpakurl");
                btnRunUpdate.IsEnabled = true;
            }
        }
        #endregion

        #region Background workers threads
        void _scriptDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            BGWorkerArgument desc = (BGWorkerArgument)e.Argument;

            WebClient cli = new WebClient();
            Stream scripts = cli.OpenRead(desc.Url);

            Dispatcher.Invoke((Action<long>)(delegate(long val)
            {
                progressBar1.Maximum = val;
            }), desc.Length);

            byte[] buffer = new byte[1024 * 64];

            string fileName = "script_update.zip";

            FileStream fs = File.Create(fileName);

            do
            {
                int byte_read = scripts.Read(buffer, 0, buffer.Length);

                if (byte_read == 0) break;

                Dispatcher.Invoke((Action<long>)(delegate(long val)
                {
                    progressBar1.Value += val;
                }), byte_read);

                fs.Write(buffer, 0, byte_read);
            }
            while (true);
            fs.Close();

            this.MarkAsDone("downloadscript");

            _pProcessor.IsSQL = desc.IsSQL;
            _pProcessor.PackagePath = fileName;
            _pProcessor.WebsitePath = this.WebsitePath;
            _pProcessor.Version = this.AssetMgrVersion;
            _pProcessor.ExecutePackage();

            
        }

        void _packageDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            BGWorkerArgument desc = (BGWorkerArgument)e.Argument;

            WebClient cli = new WebClient();
            Stream scripts = cli.OpenRead(desc.Url);

            Dispatcher.Invoke((Action<long>)(delegate(long val)
            {
                progressBar1.Maximum = val;
            }), desc.Length);

            byte[] buffer = new byte[1024 * 64];

            string fileName = "website_update.zip";

            FileStream fs = File.Create(fileName);

            do
            {
                int byte_read = scripts.Read(buffer, 0, buffer.Length);

                if (byte_read == 0) break;

                Dispatcher.Invoke((Action<long>)(delegate(long val)
                {
                    progressBar1.Value += val;
                }), byte_read);

                fs.Write(buffer, 0, byte_read);
            }
            while (true);
            fs.Close();

            this.MarkAsDone("downloadpak");

            _pProcessor.IsSQL = desc.IsSQL;
            _pProcessor.PackagePath = fileName;
            _pProcessor.WebsitePath = this.WebsitePath;
            _pProcessor.Version = this.AssetMgrVersion;
            _pProcessor.ExecutePackage();
        }
        #endregion

        #region ListItems operations
        private void AddListItem(string msg, string id)
        {
            CustomListItem lstItem = new CustomListItem();
            lstItem.State = ListItemState.Downloading;
            lstItem.Message = msg;
            lstItem.ActionId = id;
            lvActions.Items.Add(lstItem);
        }

        private void MarkAsDone(string id)
        {
            foreach (object item in lvActions.Items)
            {
                CustomListItem lvITem = item as CustomListItem;
                if (lvITem.ActionId == id)
                {
                    lvITem.ChangeState(ListItemState.Ok);
                    break;
                }
            }
        }

        private void MarkAsBroken(string id)
        {
            foreach (object item in lvActions.Items)
            {
                CustomListItem lvITem = item as CustomListItem;
                if (lvITem.ActionId == id)
                {
                    lvITem.ChangeState(ListItemState.Error);
                    break;
                }
            }
        }
        #endregion

        //private void ForceUIToUpdate()
        //{
        //    DispatcherFrame frame = new DispatcherFrame();
        //    Dispatcher.CurrentDispatcher.BeginInvoke(
        //        DispatcherPriority.Render,
        //        new DispatcherOperationCallback(
        //            delegate(object parameter)
        //            {
        //                frame.Continue = false;
        //                return null;
        //            }
        //            ), null);
        //    Dispatcher.PushFrame(frame);
        //}
    }
}
