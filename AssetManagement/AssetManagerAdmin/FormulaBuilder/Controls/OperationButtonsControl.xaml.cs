using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.Controls
{
    public interface IOperationButton
    {
        string GroupName { get; }
        string Caption { get; }
        ExpressionEntry Type { get; }
        Size ButtonSize { get; }
    }

    public class OperationButton : IOperationButton
    {
        public string GroupName { get; private set; }

        public string Caption { get; private set; }

        public ExpressionEntry Type { get; private set; }

        public Size ButtonSize { get;  private set; }
        
        public OperationButton(string caption, ExpressionEntry type, Size size)
        {
            Type = type;
            Caption = string.IsNullOrEmpty(caption) ? type.Suffix + "..." + type.Postfix : caption;
            GroupName = type.Group;
            //todo: style
            ButtonSize = size;
        }
    }

    public class OperationButtonsGroup
    {
        public string GroupName { get; set; }
        public List<IOperationButton> OperationButtons { get; set; }
    }

    /// <summary>
    /// Interaction logic for OperationButtonsControl.xaml
    /// </summary>
    public partial class OperationButtonsControl : UserControl
    {
        public static readonly DependencyProperty ButtonsGroupsProperty = DependencyProperty.Register(
            "ButtonsGroups", typeof(List<OperationButtonsGroup>), typeof(OperationButtonsControl),
            new PropertyMetadata(default(List<OperationButtonsGroup>)));

        public List<OperationButtonsGroup> ButtonsGroups
        {
            get { return (List<OperationButtonsGroup>)GetValue(ButtonsGroupsProperty); }
            set { SetValue(ButtonsGroupsProperty, value); }
        }

        public event EventHandler<IOperationButton> OnButtonClick;

        public OperationButtonsControl()
        {
            InitializeComponent();
        }

        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (OnButtonClick != null)
                OnButtonClick(this, ((FrameworkElement)sender).Tag as IOperationButton);
        }
    }
}