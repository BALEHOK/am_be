using System;
using AppFramework.Core.Classes.SearchEngine.Enumerations;

namespace AssetSite.Controls.SearchControls
{
    public partial class SearchTabs : System.Web.UI.UserControl
    {
        public SearchType ActiveSearchType { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}