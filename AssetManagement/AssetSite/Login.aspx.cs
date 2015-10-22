using System;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.ConstantsEnumerators;

namespace AssetSite
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
                Response.Redirect(Request["ReturnUrl"] ?? "/");
        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            ErrorMessage.Text = "";
            if (Membership.ValidateUser(UserLogin.Text, Password.Text) &&
                !Roles.IsUserInRole(
                    UserLogin.Text.ToLower(), 
                    PredefinedRoles.OnlyPerson.ToString()))
            {
                FormsAuthentication.SetAuthCookie(UserLogin.Text, RememberMe.Checked);
                Response.Redirect(Request["ReturnUrl"] ?? FormsAuthentication.DefaultUrl);

                //var membershipUser = Membership.GetUser(UserLogin.Text) as AssetUser;
                //var identity = new AssetManagerIdentity(
                //    membershipUser.UserName,
                //    membershipUser.Asset.ID,
                //    membershipUser.GetUserRights(),
                //    membershipUser.Asset["Role"].Value,
                //    DefaultAuthenticationTypes.ApplicationCookie);

                //var ctx = Request.GetOwinContext();
                //var authManager = ctx.Authentication;
                //authManager.SignIn(new AuthenticationProperties
                //{
                //    ExpiresUtc = DateTime.Now.AddDays(30),
                //    IssuedUtc = DateTime.Now,
                //    IsPersistent = true,
                //    RedirectUri = 
                //}, identity);

                //var oauthToken = Startup.OAuthServerOptions.AccessTokenFormat.Protect(
                //    new AuthenticationTicket(identity, new AuthenticationProperties()));
                //Response.Cookies.Add(new HttpCookie("OAuthToken")
                //{
                //    Value = oauthToken,
                //    Expires = DateTime.Now.AddDays(30),
                //});
            }
            else
            {
                ErrorMessage.Text = "Invalid login or password";
            }
        }
    }
}
