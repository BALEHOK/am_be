using System;
using AppFramework.Core.Classes;

namespace AssetSite.Controls
{
    public partial class TasksPanel : System.Web.UI.UserControl
    {
        public AssetType AssetType { get; set; }
        public long AssetUID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}