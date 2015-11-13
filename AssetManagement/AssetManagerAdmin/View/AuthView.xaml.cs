using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using AppFramework.Auth;
using AssetManagerAdmin.Model;
using AssetManagerAdmin.ViewModel;
using IdentityModel.Client;
using mshtml;

namespace AssetManagerAdmin.View
{
    public partial class AuthView : UserControl
    {
        private readonly WebBrowser _loginWebView;
        private readonly WebBrowser _logoutWebView;
        private const string CallbackUrl = "oob://localhost/AMATclient";

        public ServerConfig Server { get; private set; }

        public AuthView()
        {
            _loginWebView = new WebBrowser();
            _loginWebView.Navigating += WebView_Navigating_Login;

            _logoutWebView = new WebBrowser();
            _logoutWebView.Navigating += WebView_Navigating_Logout;
            _logoutWebView.Navigated += WebView_Navigated_Logout;

            InitializeComponent();
        }

        public void Login(ServerConfig server)
        {
            Server = server;
            var request = new AuthorizeRequest(server.AuthUrl + AuthConstants.Endpoints.Authorize);

            const string requiredScopes =
                AuthConstants.Scopes.OpenId + " " + AuthConstants.Scopes.Profile + " " + AuthConstants.Scopes.WebApi;
            var loginUrl = request.CreateAuthorizeUrl("AMAT", "id_token token", requiredScopes,
                CallbackUrl, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            _loginWebView.Visibility = Visibility.Visible;
            _loginWebView.Navigate(new Uri(loginUrl));
            WebView.Content = _loginWebView;
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

                ((AuthViewModel) DataContext).OnLoggedIn(Server, authorizeResponse);
            }
        }

        public void Logout(string authUrl, UserInfo currentUser)
        {
            const string logoutUrlFormat =
                "{0}" + AuthConstants.Endpoints.Logout + "?id_token_hint={1}&post_logout_redirect_uri=" + CallbackUrl;

            var logoutUrl = string.Format(logoutUrlFormat, authUrl, currentUser.IdToken);

            _logoutWebView.Visibility = Visibility.Visible;
            _logoutWebView.Navigate(new Uri(logoutUrl));
            WebView.Content = _logoutWebView;
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