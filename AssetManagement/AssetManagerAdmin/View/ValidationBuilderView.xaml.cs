using System.Text;
using System.Windows;
using System.Windows.Controls;
using AppFramework.Core.Classes;
using AssetManagerAdmin.ViewModel;

namespace AssetManagerAdmin.View
{
    /// <summary>
    /// Interaction logic for ValidationBuilderView.xaml
    /// </summary>
    public partial class ValidationBuilderView : UserControl
    {
        private IValidationBuilderViewModel _vm;

        public ValidationBuilderView()
        {
            InitializeComponent();
            TxtValidationExpression.SelectionChanged += SelectionChanged;            
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataContextProperty)
            {
                _vm = ((IValidationBuilderViewModel)DataContext);
                _vm.OnNewOperator += OnOnNewOperator;
            }
        }

        private void OnOnNewOperator(object sender, string operatorText)
        {
            var selectionStart = TxtValidationExpression.SelectionStart;
            var selectionLength = TxtValidationExpression.SelectionLength;            

            if (operatorText == "()")
            {
                var brackets = new StringBuilder(_vm.ValidationExpression);
                brackets.Insert(selectionStart, '(');
                brackets.Insert(selectionStart + selectionLength + 1, ')');
                _vm.ValidationExpression = brackets.ToString();
            }
            else
            {
                if (selectionLength > 0)
                {
                    _vm.ValidationExpression = _vm.ValidationExpression.Remove(selectionStart, selectionLength);
                    TxtValidationExpression.SelectionStart = selectionStart;
                }

                _vm.ValidationExpression = _vm.ValidationExpression.Insert(selectionStart, " " + operatorText + " ");

                var sb = new StringBuilder();
                // +1 is for space after operator
                var position = selectionStart + operatorText.Length + 1;
                _vm.ValidationExpression.Trim().ToCharArray().ForEachWithIndex((c, i) =>
                {
                    if (c == ' ' && sb[sb.Length - 1] == ' ')
                    {
                        if (i <= selectionStart)
                            position--;
                        return;
                    }
                    sb.Append(c);
                });

                _vm.ValidationExpression = sb.ToString();
                TxtValidationExpression.SelectionStart = position;
            }

            TxtValidationExpression.Focus();
        }

        private void SelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            PositionLabel.Content = "Symbol: " + TxtValidationExpression.SelectionStart;
        }
    }
}