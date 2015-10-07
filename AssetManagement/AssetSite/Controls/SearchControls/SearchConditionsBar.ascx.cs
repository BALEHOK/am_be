using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AssetSite.Controls.SearchControls
{
    public partial class SearchConditionsBar : System.Web.UI.UserControl
    {
        protected string SearchId { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["SearchId"] != null && Session[Constants.SearchParameters] != null)
            {
                SearchId = Request["SearchId"];
                backToSearchButton.NavigateUrl = Session[Constants.SearchParameters].ToString();
            }
            else
            {
                this.Visible = false;
            }
        }
    }
}