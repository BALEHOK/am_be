using System;
using AppFramework.Core.Classes;

namespace AssetSite
{
    public partial class _Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ApplicationSettings.ShowConfigurationPage)
            {
                Response.Redirect("~/Configuration/");
            }
            else
            {
                Response.Redirect("~/admin/");
            }
        }
    }
}
