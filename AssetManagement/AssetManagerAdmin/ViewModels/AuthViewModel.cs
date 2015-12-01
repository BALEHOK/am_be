using System;
using System.Text;
using AppFramework.Auth;
using AssetManager.Infrastructure.Models;
using AssetManagerAdmin.Model;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using AssetManagerAdmin.Infrastructure;

namespace AssetManagerAdmin.ViewModels
{
    public class AuthViewModel : ToolkitViewModelBase
    {
        public event Action<ServerConfig> OnLoggingIn;

        public event Action<string, UserInfo> OnLoggingOut;

        public UserInfo CurrentUser { get; set; }

        public ServerConfig SelectedServer { get; private set; }

        public AuthViewModel(IAppContext context)
            : base (context)
        {
            OnNavigated += (parameter) =>
            {
                if (parameter is ServerConfig && OnLoggingIn != null)
                {
                    OnLoggingIn((ServerConfig)parameter);
                    SelectedServer = (ServerConfig)parameter;
                }

                // when navigated to logout, raise event to notify the view
                if (parameter is string && (string)parameter == AppActions.LoggingOut && OnLoggingOut != null)
                    OnLoggingOut(Context.CurrentServer.AuthUrl, Context.CurrentUser);
            };
        }

        public void OnLoggedIn(AuthorizeResponse authorizeResponse)
        {
            CurrentUser = _createUser(authorizeResponse);
            MessengerInstance.Send(
                new LoginDoneModel
                {
                    Server = SelectedServer,
                    User = CurrentUser
                }, 
                AppActions.LoginDone);
        }

        public void OnLoggedOut()
        {
            MessengerInstance.Send(SelectedServer, AppActions.LogoutDone);
            NavigationService.NavigateTo(ViewModelLocator.LoginViewKey);
        }

        private UserInfo _createUser(AuthorizeResponse authorizeResponse)
        {
            var parsedToken = ParseJwt(authorizeResponse.IdentityToken);

            var user = new User
            {
                UserName = parsedToken[AuthConstants.ClaimTypes.UserName].ToString(),
                Email = parsedToken[AuthConstants.ClaimTypes.Email].ToString(),
                LastLogin = DateTime.Parse(parsedToken[AuthConstants.ClaimTypes.LastLogin].ToString()),
                UserRole = parsedToken[AuthConstants.ClaimTypes.UserRole].ToString(),
                UserRights = parsedToken[AuthConstants.ClaimTypes.UserRights].ToString()
            };

            return new UserInfo(user, authorizeResponse.AccessToken, authorizeResponse.IdentityToken);
        }

        private JObject ParseJwt(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ApplicationException("Token must have three parts separated by '.' characters.");
            }

            var part = Encoding.UTF8.GetString(Base64Url.Decode(parts[1]));

            return JObject.Parse(part);
        }
    }
}