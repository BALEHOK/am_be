using System.Collections.Generic;
using System.Linq;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using AssetManagerAdmin.Infrastructure;

namespace AssetManagerAdmin.ViewModels
{
    public class ReportsBuilderViewModel : ToolkitViewModelBase
    {
        private readonly IDataService _dataService;
        private bool _isPublishing;
        private List<CustomReportModel> _allReports;

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

        public const string SelectedAssetTypePropertyName = "SelectedAssetType";
        private AssetTypeModel _selectedAssetType;

        public AssetTypeModel SelectedAssetType
        {
            get { return _selectedAssetType; }

            set
            {
                _selectedAssetType = value;
                RaisePropertyChanged(SelectedAssetTypePropertyName);
            }
        }

        public const string ReportsListPropertyName = "ReportsList";
        private List<CustomReportModel> _reportsList;

        public List<CustomReportModel> ReportsList
        {
            get { return _reportsList; }

            set
            {
                _reportsList = value;
                RaisePropertyChanged(ReportsListPropertyName);
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

        public const string ReportFileNamePropertyName = "ReportFileName";
        private string _reportFileName;

        public string ReportFileName
        {
            get { return _reportFileName; }

            set
            {
                _reportFileName = value;
                RaisePropertyChanged(ReportFileNamePropertyName);
            }
        }

        private RelayCommand _selectReportFileCommand;

        public RelayCommand SelectReportFileCommand
        {
            get
            {
                return _selectReportFileCommand ??
                       (_selectReportFileCommand = new RelayCommand(ExecuteSelectReportFileCommand, () => true));
            }
        }

        private void ExecuteSelectReportFileCommand()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".repx",
                Filter = "Reports (.repx)|*.repx"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                ReportFileName = dlg.FileName;
            }
        }

        private RelayCommand _publishReportCommand;

        public RelayCommand PublishReportCommand
        {
            get
            {
                return _publishReportCommand ??
                       (_publishReportCommand = new RelayCommand(ExecutePublishReportCommand, () =>
                           !string.IsNullOrEmpty(ReportFileName) &&
                           !string.IsNullOrEmpty(ReportName) &&
                           ReportAssetType != null &&
                           ReportAssetType.Id > 0 &&
                           !_isPublishing));
            }
        }

        private void ExecutePublishReportCommand()
        {
            var fileName = ReportFileName;
            _isPublishing = true;
            Api.PublishReport(ReportName, fileName, ReportAssetType.Id).ContinueWith(r =>
            {
                Api.GetReportsList().ContinueWith(e =>
                {
                    ReportsList = e.Result;
                    ReportName = ReportFileName = string.Empty;
                    _isPublishing = false;
                });
            });
        }

        private RelayCommand<CustomReportModel> _viewReportCommand;

        public RelayCommand<CustomReportModel> ViewReportCommand
        {
            get
            {
                return _viewReportCommand ??
                       (_viewReportCommand =
                           new RelayCommand<CustomReportModel>(ExecuteViewReportCommand, model => true));
            }
        }

        private void ExecuteViewReportCommand(CustomReportModel reportModel)
        {
            
        }

        private RelayCommand<CustomReportModel> _deleteReportCommand;

        public RelayCommand<CustomReportModel> DeleteReportCommand
        {
            get
            {
                return _deleteReportCommand ??
                       (_deleteReportCommand =
                           new RelayCommand<CustomReportModel>(ExecuteDeleteReportCommand, model => true));
            }
        }

        private void ExecuteDeleteReportCommand(CustomReportModel reportModel)
        {
            Api.DeleteReport(reportModel.Name, reportModel.AssetTypeId).ContinueWith(e =>
            {
                _allReports.Remove(reportModel);
                RaisePropertyChanged(ReportAssetTypePropertyName);
            });
        }

        protected override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName.Equals(ReportAssetTypePropertyName))
            {
                ReportsList = ReportAssetType == null || ReportAssetType.Id < 0
                    ? _allReports
                    : _allReports.Where(r => r.AssetTypeId == ReportAssetType.Id).ToList();
            }
        }

        public ReportsBuilderViewModel(IDataService dataService, IAppContext context)
            : base(context)
        {
            _dataService = dataService;
            LoadReportsAndTypes();
        }

        private void LoadReportsAndTypes()
        {
            Api.GetReportsList().ContinueWith(task =>
            {
                _allReports = task.Result;
                RaisePropertyChanged(ReportAssetTypePropertyName);
            });

            _dataService.GetTypesInfo(Context.CurrentUser, Context.CurrentServer.ApiUrl)
                .ContinueWith(result =>
            {
                // do not modify original collection
                AssetTypesList = result.Result.ActiveTypes.ToList();
                AssetTypesList.Insert(0, new AssetTypeModel
                {
                    Id = -1,
                    DisplayName = "[All]"
                });
                ReportAssetType = AssetTypesList[0];
            });
        }
    }
}