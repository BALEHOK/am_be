using System;

namespace AssetSite.Controls
{
    public partial class RestoreAssetMessage : System.Web.UI.UserControl
    {
        public string CreationTime { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CreationTime))
                lblCreationTime.Text = CreationTime;
        }
    }
}