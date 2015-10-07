using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.Controls
{
    /// <summary>
    /// Interaction logic for DirectionSelectorControl.xaml
    /// </summary>
    public partial class DirectionSelectorControl : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(DirectionSelectorControl), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty ConnectorVisibilityProperty = DependencyProperty.Register(
            "ConnectorVisibility", typeof (Visibility), typeof (DirectionSelectorControl), new PropertyMetadata(default(Visibility)));

        public Visibility ConnectorVisibility
        {
            get { return (Visibility) GetValue(ConnectorVisibilityProperty); }
            set { SetValue(ConnectorVisibilityProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction", typeof(OperandPosition), typeof(DirectionSelectorControl), new PropertyMetadata(default(OperandPosition)));

        public OperandPosition Direction
        {
            get { return (OperandPosition)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty ArrowTemplateProperty = DependencyProperty.Register(
            "ArrowTemplate", typeof(ControlTemplate), typeof(DirectionSelectorControl), new PropertyMetadata(default(ControlTemplate)));

        public ControlTemplate ArrowTemplate
        {
            get { return (ControlTemplate)GetValue(ArrowTemplateProperty); }
            set { SetValue(ArrowTemplateProperty, value); }
        }


        public static readonly DependencyProperty ArrowColorProperty = DependencyProperty.Register(
            "ArrowColor", typeof(Brush), typeof(DirectionSelectorControl), new PropertyMetadata(default(Brush)));

        public Brush ArrowColor
        {
            get { return (Brush)GetValue(ArrowColorProperty); }
            set { SetValue(ArrowColorProperty, value); }
        }

        #endregion

        public DirectionSelectorControl()
        {
            InitializeComponent();

            ArrowColor = GetRowColor();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            ArrowColor = GetRowColor();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ArrowColor = GetRowColor();
        }

        private Brush GetRowColor()
        {
            return IsSelected
                ? ConnectorButton.IsMouseOver ? Brushes.DarkBlue : Brushes.RoyalBlue
                : ConnectorButton.IsMouseOver ? Brushes.DarkGray : Brushes.LightGray;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsSelectedProperty)
            {
                ArrowColor = GetRowColor();
            }
        }

        private void ConnectorButtonOnClick(object sender, RoutedEventArgs e)
        {
            IsSelected = !IsSelected;
        }
    }
}
