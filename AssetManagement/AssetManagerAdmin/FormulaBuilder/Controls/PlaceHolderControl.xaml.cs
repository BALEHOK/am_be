using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.Controls
{
    /// <summary>
    /// Interaction logic for PlaceHolderControl.xaml
    /// </summary>
    public partial class PlaceHolderControl : UserControl
    {
        #region Dependency properties

        public static readonly DependencyProperty ValueEditorControlProperty = DependencyProperty.Register(
            "ValueEditorControl", typeof(Control), typeof(PlaceHolderControl), new PropertyMetadata(default(Control)));

        public Control ValueEditorControl
        {
            get { return (Control)GetValue(ValueEditorControlProperty); }
            set { SetValue(ValueEditorControlProperty, value); }
        }

        #endregion

        private ExpressionEntry Entry
        {
            get { return DataContext as ExpressionEntry; }
        }

        public PlaceHolderControl()
        {
            InitializeComponent();

            PlhBorder.BorderBrush = Brushes.Transparent;
        }

        private void PlaceholderOnMouseEnter(object sender, MouseEventArgs e)
        {
            PlhBorder.BorderBrush = Brushes.Crimson;
        }

        private void PlaceholderOnMouseLeave(object sender, MouseEventArgs e)
        {
            PlhBorder.BorderBrush = Brushes.Transparent;
        }

        private void PlaceHolderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Entry != null)
            {
                Entry.Builder.ToggleSelection(Entry);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataContextProperty)
            {
                Visibility = Entry == null ? Visibility.Collapsed : Visibility.Visible;

                if (Entry == null)
                {
                    ValueEditorControl = null;
                }
                else
                {
                    ValueEditorControl = (Control) Activator.CreateInstance(Entry.EditorType);
                    ValueEditorControl.DataContext = DataContext;
                }
            }
        }

        private void PlaceHolderViewOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Keyboard.Focus(sender as UserControl);
            ((Control)sender).Focus();
        }
    }
}