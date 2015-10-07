using System;
using AppFramework.Core.Classes;

namespace AssetSite.Controls
{
    public partial class ScreensPanel : System.Web.UI.UserControl
    {
        public AssetType AssetType { get; set; }

        public AppFramework.Core.Classes.Asset Asset { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}