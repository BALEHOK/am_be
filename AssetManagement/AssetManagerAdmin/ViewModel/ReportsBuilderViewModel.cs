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
        public ServerConfig CurrentServer { get; private set; }

        public UserInfo CurrentUser { get; private set; }

        private readonly IDataService _dataService;
        private readonly IAssetsApiManager _assetsApiManager;
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
            var api = _assetsApiManager.GetAssetApi(CurrentServer.ApiUrl, CurrentUser);
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
            var api = _assetsApiManager.GetAssetApi(CurrentServer.ApiUrl, CurrentUser);
            api.DeleteReport(reportModel.Name, reportModel.AssetTypeId).ContinueWith(e =>
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

        public ReportsBuilderViewModel(IDataService dataService, IAssetsApiManager assetsApiManager)
        {
            _dataService = dataService;
            _assetsApiManager = assetsApiManager;

            MessengerInstance.Register<System.Type>(this, AppActions.DataContextChanged, currentView =>
            {
                if (currentView == typeof(ReportsBuilderViewModel))
                {
                    LoadReportsAndTypes();
                }
            });

            MessengerInstance.Register<LoginDoneModel>(this, AppActions.LoginDone, model =>
            {
                CurrentServer = model.Server;
                CurrentUser = model.User;
            });
        }

        private void LoadReportsAndTypes()
        {
            var api = _assetsApiManager.GetAssetApi(CurrentServer.ApiUrl, CurrentUser);
            MessengerInstance.Send("Loading reports list...", AppActions.LoadingStarted);
            api.GetReportsList().ContinueWith(task =>
            {
                MessengerInstance.Send("", AppActions.LoadingCompleted);
                if (task.Exception != null)
                {
                    MessengerInstance.Send(
                        new StatusMessage(task.Exception));
                }
                else
                {
                    _allReports = task.Result;
                }
            });

            MessengerInstance.Send("Loading asset types...", AppActions.LoadingStarted);
            _dataService.GetTypesInfo(CurrentUser, CurrentServer.ApiUrl).ContinueWith(result =>
            {
                MessengerInstance.Send("", AppActions.LoadingCompleted);
                if (result.Exception != null)
                {
                    MessengerInstance.Send(
                        new StatusMessage(result.Exception));
                }
                else
                {
                    // do not modify original collection
                    AssetTypesList = result.Result.ActiveTypes.ToList();
                    AssetTypesList.Insert(0, new AssetTypeModel
                    {
                        Id = -1,
                        DisplayName = "[All]"
                    });
                    ReportAssetType = AssetTypesList[0];
                }
            });
        }
    }
}