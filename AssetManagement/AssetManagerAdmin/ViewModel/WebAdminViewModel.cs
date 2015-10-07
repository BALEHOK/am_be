using AssetManagerAdmin.Model;
using AssetManagerAdmin.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using mshtml;

namespace AssetManagerAdmin.ViewModel
{
    public class WebAdminViewModel : ViewModelBase, ICommonViewModel
    {
        private readonly IDataService _dataService;
        private RelayCommand _mainMenuCommand;
        private string _server;
        private bool _isLoggingIn;

        public bool IsActive { get; set; }

        #region Properties

        public const string BrowserUrlPropertyName = "BrowserUrl";
        private string _browserUrl;

        public string BrowserUrl
        {
            get { return _browserUrl; }

            set
            {
                _browserUrl = value;
                RaisePropertyChanged(BrowserUrlPropertyName);
            }
        }

        #endregion

        #region Commands

        public RelayCommand MainMenuCommand
        {
            get
            {
                return _mainMenuCommand ??
                       (_mainMenuCommand = new RelayCommand(GoHome, () => true));
            }
        }

        private RelayCommand _handleBrowserErrorCommand;

        public RelayCommand HandleBrowserErrorCommand
        {
            get
            {
                return _handleBrowserErrorCommand ??
                       (_handleBrowserErrorCommand = new RelayCommand(ExecuteHandleBrowserErrorCommand, () => true));
            }
        }

        private void ExecuteHandleBrowserErrorCommand()
        {
            //GoHome();
        }

        private RelayCommand<PageLoadedEventArgs> _handlePageLoadCommand;        

        public RelayCommand<PageLoadedEventArgs> HandlePageLoadCommand
        {
            get
            {
                return _handlePageLoadCommand ??
                       (_handlePageLoadCommand =
                           new RelayCommand<PageLoadedEventArgs>(ExecuteHandlePageLoadCommand, args => true));
            }
        }

        private void ExecuteHandlePageLoadCommand(PageLoadedEventArgs args)
        {
            var user = _dataService.CurrentUser;
            if (user != null)
                AutoLogin(args.Document, user);
        }

        #endregion

        public WebAdminViewModel(IDataService dataService)
        {
            _dataService = dataService;
            MessengerInstance.Register<ServerConfig>(this, AppActions.LoginDone, server =>
            {
                _server = server.AdminUrl;
                if (_dataService.CurrentUser.UserModel.UserRole == "Administrators" ||
                    _dataService.CurrentUser.UserModel.UserRole == "SuperUser")
                    GoHome();
            });
            MessengerInstance.Register<object>(this, AppActions.LogoutDone, obj =>
            {
                BrowserUrl = string.Format("{0}/logout", _server);
            });
        }

        private void GoHome()
        {
            if (string.IsNullOrEmpty(_server))
                return;

            BrowserUrl = null;
            BrowserUrl = string.Format("{0}/admin/", _server);
        }

        private void AutoLogin(HTMLDocument document, UserInfo user)
        {
            if (_isLoggingIn)
            {
                _isLoggingIn = false;
                GoHome();
            }
            else
            {
                var loginName =
                    document.getElementById("ctl00_ctl00_PlaceHolderMainContent_PlaceHolderMiddleColumn_UserLogin");
                if (loginName != null)
                {
                    _isLoggingIn = true;
                    loginName.innerText = user.UserModel.UserName;
                    document.getElementById("ctl00_ctl00_PlaceHolderMainContent_PlaceHolderMiddleColumn_Password")
                        .innerText = user.Password;
                    document.getElementById("ctl00_ctl00_PlaceHolderMainContent_PlaceHolderMiddleColumn_Submit").click();
                }
            }
        }
    }
}