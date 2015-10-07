using System;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AssetSite.Helpers;

namespace AssetSite
{
    public partial class Faq : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Repeater1.DataSource = AssetsService.GetFaqItems(System.Globalization.CultureInfo.GetCultureInfo(CookieWrapper.Language));
            Repeater1.DataBind();
        }
    }
}