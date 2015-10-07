using System;
using System.Web;
using AppFramework.Core.Classes;
using AssetSite.Helpers;
using AppFramework.ConstantsEnumerators;
using System.Web.UI.WebControls;

namespace AssetSite.Controls
{
    public partial class TopMenu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ApplicationSettings.ApplicationType == ApplicationType.SOBenBUB)
            {
                menuDocuments.Visible = false;
                menuFinancial.Visible = false;
                menuLending.Visible = false;
            }
            else if (ApplicationSettings.ApplicationType == ApplicationType.AssetManager)
            {
                menuTasks.Visible = false;
            }

            if (Request.Url.PathAndQuery.StartsWith("/admin"))
                langDropdown.Visible = false;
        }

        protected void langDropdown_DataBound(object sender, EventArgs e)
        {
            string selectvalue = string.Empty;
            foreach (ListItem item in langDropdown.Items)
            {
                if (item.Value.ToLower().StartsWith(CookieWrapper.Language))
                {
                    selectvalue = item.Value;
                }
            }
            selectvalue = !string.IsNullOrEmpty(selectvalue) ? selectvalue : CookieWrapper.Language;
            langDropdown.SelectedValue = selectvalue;
        }

        protected void Language_Changed(object sender, EventArgs e)
        {
            if (!CookieWrapper.IsSet) CookieWrapper.SetCookie();
            CookieWrapper.Language = langDropdown.SelectedValue;
            Response.Redirect(Request.Url.OriginalString);
        }
    }
}