using System.Linq;
using System.Windows;
using System.Windows.Controls;

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