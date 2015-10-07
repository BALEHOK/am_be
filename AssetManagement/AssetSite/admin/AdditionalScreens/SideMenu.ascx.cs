using System;
using System.Web.UI.WebControls;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class SideMenu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 5; i++)
            {
                var menuItem = (HyperLink)FindControl(string.Format("HyperLink{0}", i));
                menuItem.NavigateUrl =
                    string.Format("~/admin/AdditionalScreens/AdditionalScreenStep{0}.aspx?ScreenId={1}&atuid={2}",
                    i, Request["ScreenId"], Request["atuid"]);
                menuItem.CssClass = Request.Url.AbsolutePath.EndsWith(string.Format("AdditionalScreenStep{0}.aspx", i))
                    ? "active_lnk"
                    : "";
            }
        }
    }
}