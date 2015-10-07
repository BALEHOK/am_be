using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;
using AssetManagerAdmin.FormulaBuilder;
using AssetManagerAdmin.ViewModel;
using NCalc.Domain;

namespace AssetManagerAdmin.View
{
    /// <summary>
    /// Interaction logic for FormulaBuilderView.xaml
    /// </summary>
    public partial class FormulaBuilderView : UserControl
    {
        public FormulaBuilderView()
        {            
            InitializeComponent();
#if DEBUG_MODE
            ButtonPaste.Visibility = Visibility.Visible;
#endif
        }
    }
}