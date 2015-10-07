using System;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;

namespace AssetSite.Search
{
    public partial class Search : SearchPage
    {
        protected void Button1_Click(object sender, EventArgs e)
        {
            TimePeriodForSearch period = rbActive.SelectedValue == "1" ? TimePeriodForSearch.CurrentTime : TimePeriodForSearch.History;
            if (tbSearch.Text.Trim() == string.Empty)
            {
                Response.Redirect("SimpleSearchKeywords.aspx");
            }
            else
            {
                var query =
                    "?Params=" +
                    Server.UrlEncode(tbSearch.Text.Trim()) +
                    "&Time=" + (int)period;

                Session[Constants.SearchParameters] = Request.Url.GetLeftPart(UriPartial.Path).Replace("Search.aspx", "SimpleSearchKeywords.aspx") + query;
                Response.Redirect("~/Search/SimpleSearchKeywords.aspx" + query);
            }
        }
    }
}
