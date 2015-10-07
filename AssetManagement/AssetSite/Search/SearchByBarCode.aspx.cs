using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AssetSite.Search
{
    public partial class SearchByBarCode : SearchPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (tbSearch.Text.Trim() == string.Empty)
                Response.Redirect("ResultByBarCode.aspx");
            else
                Response.Redirect("ResultByBarCode.aspx?Params=" + tbSearch.Text.Trim());
        }
    }
}
