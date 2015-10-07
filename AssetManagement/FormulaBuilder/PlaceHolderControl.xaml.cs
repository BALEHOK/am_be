using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FormulaBuilder
{
    /// <summary>
    /// Interaction logic for PlaceHolderControl.xaml
    /// </summary>
    public partial class PlaceHolderControl : UserControl
    {

        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register(
            "BorderColor", typeof(Brush), typeof(PlaceHolderControl), new PropertyMetadata(default(Brush)));

        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public bool IsSelected { get; set; }

        public PlaceHolderControl()
        {
            InitializeComponent();
            PlhBorder.BorderBrush = Brushes.Gold;
        }

        private void TStack_OnMouseEnter(object sender, MouseEventArgs e)
        {            
            PlhBorder.BorderBrush = Brushes.Crimson;
        }

        private void TStack_OnMouseLeave(object sender, MouseEventArgs e)
        {
            PlhBorder.BorderBrush = Brushes.Gold;
        }
    }
}
