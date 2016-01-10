using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight.Command;
using AssetManagerAdmin.Infrastructure;

namespace AssetManagerAdmin.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ToolkitViewModelBase
    {
        private readonly IDataService _dataService;

        #region Properties

        public const string IsLoadingPropertyName = "IsLoading";
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
                RaisePropertyChanged(IsLoadingPropertyName);
            }
        }

        public const string LoadingTextPropertyName = "LoadingText";
        private string _loadingText = "Please wait...";

        public string LoadingText
        {
            get { return _loadingText; }

            set
            {
                _loadingText = value;
                RaisePropertyChanged(LoadingTextPropertyName);
            }
        }

        public const string MainMenuItemsName = "MainMenuItems";
        private List<MenuItemViewModel> _mainMenuItems;

        public List<MenuItemViewModel> MainMenuItems
        {
            get { return _mainMenuItems; }
            set
            {
                _mainMenuItems = value;
                RaisePropertyChanged(MainMenuItemsName);
            }
        }

        public const string SelectedMenuItemName = "SelectedMenuItem";
        private MenuItemViewModel _selectedMenuItem;

        public MenuItemViewModel SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;
                RaisePropertyChanged(SelectedMenuItemName);
            }
        }

        public const string ServersListPropertyName = "ServersList";
        private List<string> _serversList;

        public List<string> ServersList
        {
            get { return _serversList; }

            set
            {
                _serversList = value;
                RaisePropertyChanged(ServersListPropertyName);
            }
        }
                
        public const string TypeInfoListPropertyName = "TypeInfoList";
        private List<AssetTypeModel> _typeInfoList;

        public List<AssetTypeModel> TypeInfoList
        {
            get { return _typeInfoList; }

            set
            {
                _typeInfoList = value;
                RaisePropertyChanged(TypeInfoListPropertyName);
            }
        }

        public const string SelectedTypePropertyName = "SelectedType";
        private AssetTypeModel _selectedType;

        public AssetTypeModel SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                RaisePropertyChanged(SelectedTypePropertyName);
            }
        }

        public const string SelectedAttributePropertyName = "SelectedAttribute";
        private AttributeTypeModel _selectedAttribute;

        public AttributeTypeModel SelectedAttribute
        {
            get { return _selectedAttribute; }

            set
            {
                _selectedAttribute = value;
                RaisePropertyChanged(SelectedAttributePropertyName);
            }
        }

        public const string CurrentViewPropertyName = "CurrentView";
        private ContentControl _currentView;

        public ContentControl CurrentView
        {
            get { return _currentView; }

            set
            {
                _currentView = value;
                RaisePropertyChanged(CurrentViewPropertyName);
            }
        }

        public const string IsMenuVisiblePropertyName = "IsMenuVisible";
        private bool _isMenuVisible = false;

        public bool IsMenuVisible
        {
            get { return _isMenuVisible; }

            set
            {
                _isMenuVisible = value;
                RaisePropertyChanged(IsMenuVisiblePropertyName);
            }
        }

        public const string IsAttributeSelectorEnabledPropertyName = "IsAttributeSelectorEnabled";
        private bool _isAttributeSelectorEnabled;

        public bool IsAttributeSelectorEnabled
        {
            get { return _isAttributeSelectorEnabled; }

            set
            {
                _isAttributeSelectorEnabled = value;
                RaisePropertyChanged(IsAttributeSelectorEnabledPropertyName);
            }
        }

        #endregion

        #region Commands

        private RelayCommand _openSiteCommand;

        public RelayCommand OpenSiteCommand
        {
            get
            {
                return _openSiteCommand ??
                       (_openSiteCommand = new RelayCommand(ExecuteOpenSiteCommand, () => true));
            }
        }

        private void ExecuteOpenSiteCommand()
        {
            Process.Start(new ProcessStartInfo(Context.CurrentServer.AdminUrl));
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            IDataService dataService, 
            IAppContext context,
            IDialogService dialogService)
            : base(context)
        {
            _dataService = dataService;

            MessengerInstance.Register<LoginDoneModel>(this, AppActions.LoginDone, model =>
            {
                var menuItems = _dataService.GetMainMenuItems(Context.CurrentUser);
                MainMenuItems = menuItems;
                SelectedMenuItem = MainMenuItems.First();
                IsMenuVisible = true;
                NavigationService.NavigateTo(ViewModelLocator.ReportBuilderKey);
            });

            MessengerInstance.Register<ServerConfig>(this, AppActions.LogoutDone, (server) =>
            {
                IsMenuVisible = false;
            });

            MessengerInstance.Register<StatusMessage>(this, msg => 
            {
                dialogService.ShowMessage(
                    msg.Message,
                    msg.Title,
                    msg.Status);
            });

            MessengerInstance.Register<string>(this, AppActions.LoadingStarted, message => 
            {
                IsLoading = true;
                LoadingText = message;
            });

            MessengerInstance.Register<string>(this, AppActions.LoadingCompleted, (msg) => 
            {
                IsLoading = false;
            });
        }
    }
}