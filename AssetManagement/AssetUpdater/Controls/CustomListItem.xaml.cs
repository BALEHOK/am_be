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

namespace AssetUpdater.Controls
{
    /// <summary>
    /// Interaction logic for CustomListItem.xaml
    /// </summary>
    public partial class CustomListItem : UserControl
    {
        public ListItemState State { get; set; }

        public string Message { get; set; }

        public string ActionId { get; set; }

        public CustomListItem()
        {
            try
            {
                InitializeComponent();
                this.State = ListItemState.Downloading;
            }
            catch { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DrawImage();
        }

        public void ChangeState(ListItemState state)
        {
            this.State = state;
            this.DrawImage();
        }

        private void DrawImage()
        {
            Dispatcher.Invoke((Action)(delegate()
            {
                if (this.State == ListItemState.Downloading)
                {
                    image1.Source = (ImageSource)FindResource("downloadImg");
                }
                if (this.State == ListItemState.Ok)
                {
                    image1.Source = (ImageSource)FindResource("okImage");
                    textBox1.Text += " Done";
                }
                if (this.State == ListItemState.Error)
                {
                    image1.Source = (ImageSource)FindResource("errorImage");
                    textBox1.Text += " Error";
                }
            }));
        }
    }
}
