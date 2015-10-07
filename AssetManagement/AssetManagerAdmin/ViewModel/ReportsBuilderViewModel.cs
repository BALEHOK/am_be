using System.Collections.Generic;
using System.Linq;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.Model;
using AssetManagerAdmin.WebApi;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace AssetManagerAdmin.ViewModel
{
    public class ReportsBuilderViewModel : ViewModelBase
    {
        private string _server;
        private readonly IDataService _dataService;
        private readonly IAssetsApiManager _assetsApiManager;
        private bool _isPublishing;
        private List<CustomReportModel> _allReports;

        #region Properties

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

        #endregion

        #region Commands

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
            var api = _assetsApiManager.GetAssetApi(_server, _dataService.CurrentUser);
            var fileName = ReportFileName;
            _isPublishing = true;
            api.PublishReport(ReportName, fileName, ReportAssetType.Id).ContinueWith(r =>
            {
                api.GetReportsList().ContinueWith(e =>
                {
                    ReportsList = e.Result;
                    ReportName = ReportFileName = string.Empty;
                    _isPublishing = false;
                });
            });
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
            var api = _assetsApiManager.GetAssetApi(_server, _dataService.CurrentUser);
            api.DeleteReport(reportModel.Name, reportModel.AssetTypeId).ContinueWith(e =>
            {
                _allReports.Remove(reportModel);
                RaisePropertyChanged(ReportAssetTypePropertyName);
            });
        }

        #endregion

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

        public ReportsBuilderViewModel(IDataService dataService, IAssetsApiManager assetsApiManager)
        {
            _dataService = dataService;
            _assetsApiManager = assetsApiManager;

            MessengerInstance.Register<ServerConfig>(this, AppActions.LoginDone, server =>
            {
                _server = server.ApiUrl;
                var api = _assetsApiManager.GetAssetApi(_server, _dataService.CurrentUser);
                api.GetReportsList().ContinueWith(task => { _allReports = task.Result; });

                _dataService.GetTypesInfo(_server, (typesInfo, e) =>
                {
                    // do not modify original collection
                    AssetTypesList = typesInfo.ActiveTypes.ToList();
                    AssetTypesList.Insert(0, new AssetTypeModel
                    {
                        Id = -1,
                        DisplayName = "[All]"
                    });
                    ReportAssetType = AssetTypesList[0];
                });
            });
        }
    }
}