using System;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;

namespace AssetSite.Search
{
    public partial class NewResultByDocument : SearchResultPage
    {
        protected override void ProcessRequest()
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["Params"] != null)
                {
                    tbSearch.Text = Request.QueryString["Params"].ToString().Trim();

                    if (Session["TaskConfig"] != null)
                    {
                        SearchConfigurationDescriptor desc = new SearchConfigurationDescriptor();
                        desc.SearchType = AppFramework.Core.Classes.SearchEngine.Enumerations.SearchType.SearchByDocuments;
                        desc.Keywords = tbSearch.Text;
                        desc.SkipLog = Request["FromLog"] != null;
                        Session["TaskData"] = desc.Serialize();
                    }

                    int totalItems = 0;
                    SearchResult = SearchEngine.FindDocumentsByKeywords(tbSearch.Text, out totalItems, Request["FromLog"] != null);
                    throw new NotImplementedException();
                    //SearchMasterPage.ResultSet = SearchResult.Entities;
                }
            }
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchByDocuments.aspx?Params=" + tbSearch.Text.Trim());
        }

        protected override void SetSearchConditions()
        {
            SearchConditions = string.Format(tbSearch.Text.Trim());
        }
    }
}