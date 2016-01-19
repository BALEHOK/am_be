using System.Collections.Generic;
using System.Linq;
using AssetManager.Infrastructure.Models.TypeModels;
using GalaSoft.MvvmLight.Command;
using AssetManagerAdmin.Infrastructure;
using System;
using AssetManager.Infrastructure.Services;

namespace AssetManagerAdmin.ViewModels
{
    public class SaveReportDialogViewModel : ToolkitViewModelBase
    {
        public event Action OnReportCreate;

        public const string AssetTypesListPropertyName = "AssetTypesList";
        private List<AssetTypeModel> _assetTypesList;

        public List<AssetTypeModel> AssetTypesList
        {
            get { return _assetTypesList; }

            set
            {
                _assetTypesList = value;
                RaisePropertyChanged(AssetTypesListPropertyName);
            }
        }

        public const string ReportAssetTypePropertyName = "ReportAssetType";
        private AssetTypeModel _reportAssetType;

        public AssetTypeModel ReportAssetType
        {
            get { return _reportAssetType; }

            set
            {
                _reportAssetType = value;
                RaisePropertyChanged(ReportAssetTypePropertyName);
            }
        }

        public const string ReportNamePropertyName = "ReportName";
        private string _reportName;

        public string ReportName
        {
            get { return _reportName; }

            set
            {
                _reportName = value;
                RaisePropertyChanged(ReportNamePropertyName);
            }
        }

        private RelayCommand _createReportCommand;

        public RelayCommand CreateReportCommand
        {
            get
            {
                return _createReportCommand ??
                       (_createReportCommand = new RelayCommand(
                           ExecuteCreateReportCommand, CanCreateReport));
            }
        }

        private bool CanCreateReport()
        {
            return ReportAssetType != null &&
                ReportAssetType.Id.HasValue &&
                !string.IsNullOrEmpty(ReportName);                       
        }

        private void ExecuteCreateReportCommand()
        {
            if (OnReportCreate != null)
                OnReportCreate();
        }

        protected override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName.Equals(ReportAssetTypePropertyName))
                CreateReportCommand.RaiseCanExecuteChanged();

            if (propertyName.Equals(ReportNamePropertyName))
                CreateReportCommand.RaiseCanExecuteChanged();
        }

        public SaveReportDialogViewModel(
            IAssetTypeService assetTypeService, 
            IAppContext context)
            : base(context)
        {
            var assetTypes = assetTypeService.GetAssetTypes();

            // do not modify original collection
            AssetTypesList = new List<AssetTypeModel>(assetTypes.ActiveTypes);
            AssetTypesList.Insert(0, new AssetTypeModel
            {
                Id = null,
                DisplayName = "[All]"
            });
            ReportAssetType = AssetTypesList[0];
        }
    }
}