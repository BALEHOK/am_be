using System;
using AssetSite.Helpers;
using Microsoft.Practices.Unity;
using AssetManager.Infrastructure.Services;

namespace AssetSite
{
    public partial class Faq : BasePage
    {
        [Dependency]
        public IFaqService FaqService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Repeater1.DataSource = FaqService.GetFaqItems(System.Globalization.CultureInfo.GetCultureInfo(CookieWrapper.Language));
            Repeater1.DataBind();
        }
    }
}