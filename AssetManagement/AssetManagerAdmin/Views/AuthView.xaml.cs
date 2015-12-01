using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using AppFramework.Auth;
using IdentityModel.Client;
using mshtml;
using AssetManagerAdmin.ViewModels;

namespace AssetManagerAdmin.View
{
    public partial class AuthView : UserControl
    {
        private readonly WebBrowser _loginWebView;
        private readonly WebBrowser _logoutWebView;
        private const string CallbackUrl = "oob://localhost/AMATclient";
        private const string LogoutUrlFormat = "{0}" + AuthConstants.Endpoints.Logout + "?id_token_hint={1}&post_logout_redirect_uri=" + CallbackUrl;
        private const string RequiredScopes = AuthConstants.Scopes.OpenId + " " + AuthConstants.Scopes.Profile + " " + AuthConstants.Scopes.WebApi;

        public AuthView()
        {
            _loginWebView = new WebBrowser();
            _loginWebView.Navigating += WebView_Navigating_Login;

            _logoutWebView = new WebBrowser();
            _logoutWebView.Navigating += WebView_Navigating_Logout;
            _logoutWebView.Navigated += WebView_Navigated_Logout;

            DataContextChanged += (sender, args) =>
            {
                ((AuthViewModel)DataContext).OnLoggingIn += (server) =>
                {
                    var request = new AuthorizeRequest(server.AuthUrl + AuthConstants.Endpoints.Authorize);
                    var loginUrl = request.CreateAuthorizeUrl("AMAT", "id_token token", RequiredScopes,
                        CallbackUrl, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

                    _loginWebView.Visibility = Visibility.Visible;
                    _loginWebView.Navigate(new Uri(loginUrl));
                    WebView.Content = _loginWebView;
                };

                ((AuthViewModel)DataContext).OnLoggingOut += (authUrl, user) =>
                {
                    var logoutUrl = string.Format(LogoutUrlFormat, authUrl, user.IdToken);
                    _logoutWebView.Visibility = Visibility.Visible;
                    _logoutWebView.Navigate(new Uri(logoutUrl));
                    WebView.Content = _logoutWebView;
                };
            };

            InitializeComponent();
        }

        public void WebView_Navigating_Login(object sender, NavigatingCancelEventArgs e)
        {
            // ToDo [Aleksandr Shukletsov] validate authorize response
            if (e.Uri.ToString().StartsWith(CallbackUrl))
            {
                AuthorizeResponse authorizeResponse;
                if (e.Uri.AbsoluteUri.Contains("#"))
                {
                    authorizeResponse = new AuthorizeResponse(e.Uri.AbsoluteUri);
                }
                else
                {
                    var document = (IHTMLDocument3) ((WebBrowser) sender).Document;
                    var inputElements = document.getElementsByTagName("INPUT").OfType<IHTMLElement>();
                    var resultUrl = "?";

                    foreach (var input in inputElements)
                    {
                        resultUrl += input.getAttribute("name") + "=";
                        resultUrl += input.getAttribute("value") + "&";
                    }

                    resultUrl = resultUrl.TrimEnd('&');
                    authorizeResponse = new AuthorizeResponse(resultUrl);
                }

                e.Cancel = true;

                _loginWebView.Visibility = Visibility.Collapsed;

                ((AuthViewModel) DataContext).OnLoggedIn(authorizeResponse);
            }
        }

        /// <summary>
        /// handles the case when user is autoredirected to logout_redirect URL
        /// </summary>
        private void WebView_Navigating_Logout(object sender, NavigatingCancelEventArgs e)
        {
            var navigatingUrl = e.Uri.ToString();
            if (navigatingUrl.Contains("unsafe:")) // IE may add "unsafe:" prefix to "oob://" URL
            {
                navigatingUrl = navigatingUrl.Substring(7);
            }

            if (navigatingUrl.StartsWith(CallbackUrl))
            {
                e.Cancel = true;

                Logout();
            }
        }

        /// <summary>
        /// handles the case when user is redirected to ID3 logout URL
        /// </summary>
        private void WebView_Navigated_Logout(object sender, NavigationEventArgs e)
        {
            var navigatingPath = e.Uri.AbsolutePath;
            if (navigatingPath.EndsWith(AuthConstants.RoutePaths.Logout))
            {
                Logout();
            }
        }

        private void Logout()
        {
            _logoutWebView.Visibility = Visibility.Collapsed;
            ((AuthViewModel) DataContext).OnLoggedOut();
        }
    }
}