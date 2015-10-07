using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.ValueEditors
{
    /// <summary>
    /// Interaction logic for TextValueEditor.xaml
    /// </summary>
    public partial class TextValueEditor : UserControl
    {
        public TextValueEditor()
        {
            InitializeComponent();
        }

        private ExpressionEntry Entry { get { return DataContext as ExpressionEntry; } }

        private void ValueTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Entry.Value = ValueTextBox.Text;
                    Entry.IsEntrySelected = false;
                    break;
                case Key.Escape:
                    ValueTextBox.Text = Entry.Value;
                    Entry.IsEntrySelected = false;
                    break;
            }
        }
    }
}