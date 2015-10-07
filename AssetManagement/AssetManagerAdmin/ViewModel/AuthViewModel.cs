using System;
using System.Text;
using AppFramework.Auth;
using AssetManager.Infrastructure.Models;
using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace AssetManagerAdmin.ViewModel
{
    public class AuthViewModel : ViewModelBase, ICommonViewModel
    {
        private readonly IDataService _dataService;

        public AuthViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void OnLoggedIn(AuthorizeResponse authorizeResponse)
        {
            _dataService.CurrentUser = CreateUser(authorizeResponse);
            MessengerInstance.Send(_dataService.SelectedServer, AppActions.LoginDone);
        }

        private UserInfo CreateUser(AuthorizeResponse authorizeResponse)
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

        public bool IsActive { get; set; }

        public void OnLoggedOut()
        {
            _dataService.CurrentUser = null;
            MessengerInstance.Send(_dataService.SelectedServer, AppActions.LogoutDone);
        }
    }
}