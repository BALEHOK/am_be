using AssetManagerAdmin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssetManagerAdmin.View
{
    /// <summary>
    /// Interaction logic for ReportsBuilderView.xaml
    /// </summary>
    public partial class OpenReportDialog : Window
    {
        public OpenReportDialog()
        {
            InitializeComponent();
            this.DataContextChanged += OpenReportDialog_DataContextChanged;
            SetReportHandler();
        }

        private void SetReportHandler()
        {
            if (this.DataContext is OpenReportDialogViewModel)
                ((OpenReportDialogViewModel)this.DataContext).OnReportSelected += DataContext_OnReportSelected;
        }

        private void OpenReportDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var d = e.NewValue as OpenReportDialogViewModel;
            if (d == null)
                return;

            SetReportHandler();
        }

        private void DataContext_OnReportSelected()
        {
            this.DialogResult = true;
        }
    }
}
