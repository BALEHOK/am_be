﻿using AssetManagerAdmin.Infrastructure;
using AssetManagerAdmin.Model;
using AssetManagerAdmin.View;
using GalaSoft.MvvmLight.Command;
using mshtml;

namespace AssetManagerAdmin.ViewModels
{
    public class WebAdminViewModel : ToolkitViewModelBase
    {
        private RelayCommand _mainMenuCommand;
        private bool _isLoggingIn;

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
            if (Context.CurrentUser != null)
                AutoLogin(args.Document, Context.CurrentUser);
        }

        #endregion

        public WebAdminViewModel(IAppContext context)
            : base(context)
        {
            if (Context.CurrentUser.UserModel.UserRole == "Administrators" ||
                Context.CurrentUser.UserModel.UserRole == "SuperUser")
                GoHome();

            MessengerInstance.Register<object>(this, AppActions.LogoutDone, obj =>
            {
                BrowserUrl = string.Format("{0}/logout", Context.CurrentServer.AdminUrl);
            });
        }

        private void GoHome()
        {
            if (string.IsNullOrEmpty(Context.CurrentServer.AdminUrl))
                return;

            BrowserUrl = null;
            BrowserUrl = string.Format("{0}/admin/", Context.CurrentServer.AdminUrl);
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