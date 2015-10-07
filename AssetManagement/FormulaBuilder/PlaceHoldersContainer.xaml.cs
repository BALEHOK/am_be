using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for PlaceHoldersContainer.xaml
    /// </summary>
    public partial class PlaceHoldersContainer : UserControl
    {
        public PlaceHoldersContainer()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            StackPanel.Children.Clear();
        }

        public void Add(PlaceHolder placeHolder)
        {
            var p = new PlaceHolderControl { DataContext = placeHolder };
            StackPanel.Children.Add(p);
        }
    }
}