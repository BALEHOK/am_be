using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using Microsoft.Practices.Unity;
using AppFramework.Core.Classes.ScreensServices;

namespace AssetSite.Controls
{
    public partial class ScreenDetails : System.Web.UI.UserControl
    {
        [Dependency]
        public IScreensService ScreensService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                long screenId;
                if (Request["ScreenId"] != null && long.TryParse(Request["ScreenId"], out screenId))
                {

                    var screen = ScreensService.GetScreenById(screenId);
                    if (screen != null)
                    {
                        ScreenTitle.Text = new TranslatableString(screen.Title).GetTranslation();
                        ScreenSubtitle.Text = new TranslatableString(screen.Subtitle).GetTranslation();
                        ScreenText.Text = new TranslatableString(screen.PageText).GetTranslation();
                    }
                }
            }
        }
    }
}