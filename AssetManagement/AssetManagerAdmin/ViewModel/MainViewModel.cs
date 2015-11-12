using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.Model;
using AssetManagerAdmin.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace AssetManagerAdmin.ViewModel
{
    public interface ICommonViewModel
    {
        bool IsActive { get; set; }
    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, ICommonViewModel
    {
        private readonly IDataService _dataService;
        private readonly Dictionary<int, ContentControl> _views;

        public bool IsActive { get; set; }

        public IDialogService DialogService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDialogService>();
            }
        }

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
        private List<MainMenuItem> _mainMenuItems;

        public List<MainMenuItem> MainMenuItems
        {
            get { return _mainMenuItems; }
            set
            {
                _mainMenuItems = value;
                RaisePropertyChanged(MainMenuItemsName);
            }
        }

        public const string SelectedMenuItemName = "SelectedMenuItem";
        private MainMenuItem _selectedMenuItem;

        public MainMenuItem SelectedMenuItem
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

        public const string SelectedServerPropertyName = "SelectedServer";
        private ServerConfig _selectedServer;

        public ServerConfig SelectedServer
        {
            get { return _selectedServer; }

            set
            {
                _selectedServer = value;
                RaisePropertyChanged(SelectedServerPropertyName);
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

        public const string IsViewsMenuEnabledPropertyName = "IsViewsMenuEnabled";
        private bool _isViewsMenuEnabled;

        public bool IsViewsMenuEnabled
        {
            get { return _isViewsMenuEnabled; }

            set
            {
                _isViewsMenuEnabled = value;
                RaisePropertyChanged(IsViewsMenuEnabledPropertyName);
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

        private RelayCommand _logoutCommand;

        public RelayCommand LogoutCommand
        {
            get
            {
                return _logoutCommand ??
                       (_logoutCommand = new RelayCommand(ExecuteLogoutCommand, () => true));
            }
        }

        private void ExecuteLogoutCommand()
        {
            IsViewsMenuEnabled = false;
            IsAttributeSelectorEnabled = false;

            var authView = new AuthView();
            CurrentView = authView;
            authView.Logout(_dataService.SelectedServer.AuthUrl, _dataService.CurrentUser);
            MessengerInstance.Send(new object(), AppActions.LoggingOut);
        }

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
            Process.Start(new ProcessStartInfo(SelectedServer.AdminUrl));
        }

        #endregion

        protected override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName == SelectedMenuItemName && SelectedMenuItem != null)
            {
                var id = SelectedMenuItem.Id;
                CurrentView = _views.ContainsKey(id) ? _views[id] : null;

                // endable attribute selector for formula and validation builders
                IsAttributeSelectorEnabled = id == 2;

                // let viewmodels know when to start loading data
                if (CurrentView.DataContext != null)
                    MessengerInstance.Send(CurrentView.DataContext.GetType(), 
                        AppActions.DataContextChanged);
            }
            else if (propertyName == SelectedTypePropertyName)
            {
                _dataService.CurrentAssetType = SelectedType;
            }
            else if (propertyName == SelectedAttributePropertyName)
            {
                _dataService.CurrentAssetAttribute = SelectedAttribute;
                // send notification on property change
                var property = GetType().GetProperty(propertyName).GetValue(this);
                MessengerInstance.Send(property, propertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            EventManager.RegisterClassHandler(typeof(UserControl), UIElement.KeyUpEvent, new KeyEventHandler(
                (sender, args) =>
                {
                    if (args.OriginalSource is UserControl)
                        MessengerInstance.Send(args, CurrentView.DataContext.ToString());
                }));

            _views = new Dictionary<int,ContentControl>
            {
                // {0, new WebAdminView()},
                {1, new FormulaBuilderView()},
                {2, new ValidationBuilderView()},
                {3, new ReportsBuilderView()} 
            };

            _dataService = dataService;

            MessengerInstance.Register<ServerConfig>(this, AppActions.LoggingIn, server =>
            {
                var authView = new AuthView();
                CurrentView = authView;
                authView.Login(server.AuthUrl);
            });

            MessengerInstance.Register<ServerConfig>(this, AppActions.LoginDone, server =>
            {
                _dataService.GetMainMenuItems((menuItems, exception) =>
                {
                    SelectedServer = server;
                    MainMenuItems = menuItems;
                    SelectedMenuItem = MainMenuItems.First();
                    IsViewsMenuEnabled = true;
                });
            });

            MessengerInstance.Register<ServerConfig>(this, AppActions.LogoutDone, server =>
            {
                CurrentView = new LoginView();
            });

            MessengerInstance.Register<StatusMessage>(this, msg => 
            {
                DialogService.ShowMessage(
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

            IsViewsMenuEnabled = false;
            CurrentView = new LoginView();
        }
    }
}