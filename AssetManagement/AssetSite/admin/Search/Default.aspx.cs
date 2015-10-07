using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Helpers;

namespace AssetSite.admin.Search
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtExcludeWords.Text = string.Join(" ", ApplicationSettings.SearchExcludeWords);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string text = Formatting.FormatForSearch(txtExcludeWords.Text.Trim().ToLower());
            ApplicationSettings.SearchExcludeWords = text.Split(new char[] { ' ', ',' }).ToList();
            Response.Redirect("~/admin/Search/");
        }
    }
}