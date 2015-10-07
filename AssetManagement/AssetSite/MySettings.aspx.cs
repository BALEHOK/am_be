using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AssetSite.Helpers;
using System;
using System.Web;
using System.Web.Security;
using Microsoft.Practices.Unity;
using System.Collections.Generic;

namespace AssetSite
{
    public partial class MySettings : BasePage
    {
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AccessManager.Instance.ClearPersonalData();
                if (Session["save_successful"] != null)
                {
                    lblInfoMessage.Visible = true;
                    Session["save_successful"] = null;
                }
            }
            aapUserSettings.Asset = AuthenticationService.CurrentUser.Asset;
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            AppFramework.Core.Classes.Asset asset;
            IDictionary<AssetAttribute, AppFramework.Core.Classes.Asset> dependencies;

            var isValid = aapUserSettings.TryGetValidAssetWithDependencies(out asset, out dependencies);
            if (isValid)
            {
                AssetsService.InsertAsset(asset, dependencies);

                var language = AuthenticationService.CurrentUser.GetUserLanguage();
                if (!string.IsNullOrEmpty(language))
                {
                    CookieWrapper.Language = language.ToLower();
                }
                FormsIdentity id = (FormsIdentity)User.Identity;
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    HttpCookie removeCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                    removeCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(removeCookie);
                }
                FormsAuthentication.SetAuthCookie(AuthenticationService.CurrentUser.Asset.Name, id.Ticket.IsPersistent);
                AccessManager.Instance.ClearPersonalData();
                Session["save_successful"] = true;
                Response.Redirect("~/MySettings.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/default.aspx");
        }
    }
}