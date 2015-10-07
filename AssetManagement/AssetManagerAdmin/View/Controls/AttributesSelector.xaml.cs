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

namespace AssetManagerAdmin.View.Controls
{
    /// <summary>
    /// Interaction logic for AttributesSelector.xaml
    /// </summary>
    public partial class AttributesSelector : UserControl
    {
        public static readonly DependencyProperty AssetTypesListProperty = DependencyProperty.Register(
            "AssetTypesList", typeof(List<AssetTypeModel>), typeof(AttributesSelector), new PropertyMetadata(default(List<AssetTypeModel>)));

        public List<AssetTypeModel> AssetTypesList
        {
            get { return (List<AssetTypeModel>)GetValue(AssetTypesListProperty); }
            set { SetValue(AssetTypesListProperty, value); }
        }

        public static readonly DependencyProperty AssetTypeProperty = DependencyProperty.Register(
            "AssetType", typeof(AssetTypeModel), typeof(AttributesSelector), new PropertyMetadata(default(AssetTypeModel)));

        public AssetTypeModel AssetType
        {
            get { return (AssetTypeModel)GetValue(AssetTypeProperty); }
            set { SetValue(AssetTypeProperty, value); }
        }

        public static readonly DependencyProperty AttributesListProperty = DependencyProperty.Register(
            "AttributesList", typeof(List<AttributeTypeModel>), typeof(AttributesSelector), new PropertyMetadata(default(List<AttributeTypeModel>)));

        public List<AttributeTypeModel> AttributesList
        {
            get { return (List<AttributeTypeModel>)GetValue(AttributesListProperty); }
            set { SetValue(AttributesListProperty, value); }
        }

        public static readonly DependencyProperty SelectedAttributeProperty = DependencyProperty.Register(
            "SelectedAttribute", typeof(AttributeTypeModel), typeof(AttributesSelector), new PropertyMetadata(default(AttributeTypeModel)));

        public AttributeTypeModel SelectedAttribute
        {
            get { return (AttributeTypeModel)GetValue(SelectedAttributeProperty); }
            set { SetValue(SelectedAttributeProperty, value); }
        }

        public AttributesSelector()
        {
            InitializeComponent();

            AttributesListView.SelectionChanged += (sender, args) =>
            {
                SelectedAttribute = (AttributeTypeModel)AttributesListView.SelectedItem;
            };
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
                AttributesList = AssetType == null ? null : AssetType.Attributes;
                if (AttributesList != null)
                    SelectedAttribute = AttributesList.FirstOrDefault();
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