using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssetManagerAdmin.FormulaBuilder.Expressions;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.Controls
{
    /// <summary>
    /// Interaction logic for FormulaBuilderControl.xaml
    /// </summary>
    public partial class FormulaBuilderControl : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty ExpressionProperty = DependencyProperty.Register(
            "Expression", typeof(ExpressionEntry), typeof(FormulaBuilderControl),
            new PropertyMetadata(default(ExpressionEntry)));

        public ExpressionEntry Expression
        {
            get { return (ExpressionEntry)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        public static readonly DependencyProperty GrammarProperty = DependencyProperty.Register(
            "Grammar", typeof(ExpressionsGrammar), typeof(FormulaBuilderControl),
            new PropertyMetadata(default(ExpressionsGrammar)));

        public ExpressionsGrammar Grammar
        {
            get { return (ExpressionsGrammar)GetValue(GrammarProperty); }
            set { SetValue(GrammarProperty, value); }
        }

        public static readonly DependencyProperty BuilderProperty = DependencyProperty.Register(
            "Builder", typeof(ExpressionBuilder), typeof(FormulaBuilderControl),
            new PropertyMetadata(default(ExpressionBuilder)));

        public ExpressionBuilder Builder
        {
            get { return (ExpressionBuilder)GetValue(BuilderProperty); }
            set { SetValue(BuilderProperty, value); }
        }

        public static readonly DependencyProperty ExpressionParserProperty = DependencyProperty.Register(
            "ExpressionParser", typeof (ExpressionParser), typeof (FormulaBuilderControl), new PropertyMetadata(default(ExpressionParser)));

        public ExpressionParser ExpressionParser
        {
            get { return (ExpressionParser) GetValue(ExpressionParserProperty); }
            set { SetValue(ExpressionParserProperty, value); }
        }

        #endregion        

        public FormulaBuilderControl()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(Control), KeyUpEvent, new KeyEventHandler(
                (sender, args) =>
                {
                    // ignore input from text boxes
                    if (sender is TextBox)
                    {
                        args.Handled = true;
                        return;
                    }
                    
                    switch (args.Key)
                    {
                        case Key.Delete:
                            Builder.RemoveSelectedEntry();
                            args.Handled = true;
                            break;
                        case Key.Escape:
                            var selectedEntry = Builder.GetSelectedEntry();
                            Builder.ToggleSelection(selectedEntry);
                            break;
                    }
                }));
        }

        private static List<OperationButtonsGroup> GetOperationButtons(ExpressionsGrammar grammar)
        {
            if (grammar == null)
                return null;

            var buttonsGroups = grammar.Get<ExpressionEntry>()
                .GroupBy(entry => entry.Group)
                .ToDictionary(group => group.Key, list => list.ToList()).Select(group => new OperationButtonsGroup
                {
                    GroupName = group.Key,
                    OperationButtons =
                        group.Value.Select(entry => new OperationButton(entry.DisplayName, entry, new Size(85, 25)))
                            .Cast<IOperationButton>()
                            .ToList()
                }).ToList();

            return buttonsGroups;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == GrammarProperty)
            {
                OperationButtons.ButtonsGroups = GetOperationButtons(Grammar);
            }

            if (e.Property == ExpressionProperty)
            {
                RootPlaceHolder.DataContext = Expression;
            }

            if (e.Property == BuilderProperty && Builder != null)
            {
                Builder.OnExpressionChanged += (sender, args) =>
                {
                    Expression = null;
                    Expression = Builder.Expression;
                };
            }
        }

        private void OperationButtonClick(object sender, IOperationButton e)
        {
            Builder.AddEntry(e.Type);
        }

        private void ClearButtonOnClick(object sender, RoutedEventArgs e)
        {
            Builder.Reset();
        }

        private void UndoButtonOnClick(object sender, RoutedEventArgs e)
        {
            Builder.UndoLastAction();
        }

        private void CopyButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (Builder.Expression != null)
                Clipboard.SetText(Builder.Expression.ToString());
        }

        private void PasteButtonOnClick(object sender, RoutedEventArgs e)
        {
            var clipboardText = Clipboard.GetText();
            if (ExpressionParser != null)
            {
                try
                {
                    var expression = ExpressionParser.Parse(clipboardText);
                    Builder.SetRootEntry(expression);
                }
                catch (Exception)
                {
                    MessageBox.Show(string.Format("'{0}' is not a valid expression", clipboardText));
                }
            }
        }
    }
}