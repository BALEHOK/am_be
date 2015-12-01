using System;
using System.Collections.Generic;
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
using AssetManager.Infrastructure.Models.TypeModels;

//todo: combine common behaviour of selectors into single control

namespace AssetManagerAdmin.View.Controls
{
    /// <summary>
    /// Interaction logic for ScreenAttributesSelector.xaml
    /// </summary>
    public partial class ScreenAttributesSelector : UserControl
    {
        public static readonly DependencyProperty AssetTypesListProperty = DependencyProperty.Register(
            "AssetTypesList", typeof (List<AssetTypeModel>), typeof (ScreenAttributesSelector), new PropertyMetadata(default(List<AssetTypeModel>)));

        public List<AssetTypeModel> AssetTypesList
        {
            get { return (List<AssetTypeModel>) GetValue(AssetTypesListProperty); }
            set { SetValue(AssetTypesListProperty, value); }
        }

        public static readonly DependencyProperty AssetTypeProperty = DependencyProperty.Register(
            "AssetType", typeof (AssetTypeModel), typeof (ScreenAttributesSelector), new PropertyMetadata(default(AssetTypeModel)));

        public AssetTypeModel AssetType
        {
            get { return (AssetTypeModel) GetValue(AssetTypeProperty); }
            set { SetValue(AssetTypeProperty, value); }
        }

        public static readonly DependencyProperty ScreensListProperty = DependencyProperty.Register(
            "ScreensList", typeof(List<AssetTypeScreenModel>), typeof(ScreenAttributesSelector), new PropertyMetadata(default(List<AssetTypeScreenModel>)));

        public List<AssetTypeScreenModel> ScreensList
        {
            get { return (List<AssetTypeScreenModel>)GetValue(ScreensListProperty); }
            set { SetValue(ScreensListProperty, value); }
        }

        public static readonly DependencyProperty SelectedScreenAttributeProperty = DependencyProperty.Register(
            "SelectedScreenAttribute", typeof (ScreenPanelAttributeModel), typeof (ScreenAttributesSelector), new PropertyMetadata(default(ScreenPanelAttributeModel)));

        public ScreenPanelAttributeModel SelectedScreenAttribute
        {
            get { return (ScreenPanelAttributeModel) GetValue(SelectedScreenAttributeProperty); }
            set { SetValue(SelectedScreenAttributeProperty, value); }
        }

        public ScreenAttributesSelector()
        {
            InitializeComponent();            
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == AssetTypesListProperty && AssetTypesList != null)
            {
                AssetType = AssetTypesList.FirstOrDefault();
            }

            if (e.Property == AssetTypeProperty)
            {                
                ScreensList = AssetType == null ? null : AssetType.Screens;
            }
        }

        private void AttributesListView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
