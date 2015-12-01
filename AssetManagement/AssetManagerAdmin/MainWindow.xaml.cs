using AssetManagerAdmin.ViewModels;
using System.Windows;

namespace AssetManagerAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();            
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var content = MainFrame.Content as FrameworkElement;
            if (content == null)
                return;
            var contentDataContext = content.DataContext as ToolkitViewModelBase;
            if (contentDataContext == null)
                return;
            contentDataContext.RaiseNavigatedEvent();
        }
    }
}