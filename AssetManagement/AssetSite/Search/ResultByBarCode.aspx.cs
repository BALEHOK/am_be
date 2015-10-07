using System;
using AppFramework.Core.Classes.SearchEngine;

namespace AssetSite.Search
{
    public partial class NewResultByBarcode : SearchResultPage
    {
        protected override void ProcessRequest()
        {
            var @params = Request.QueryString["Params"];
            if (!IsPostBack && !string.IsNullOrWhiteSpace(@params))
            {
                tbSearch.Text = @params.Trim();

                int totalItems = 0;
                SearchResult = SearchEngine.FindByBarcode(tbSearch.Text, out totalItems, Request["FromLog"] != null);

                throw new NotImplementedException();
                //SearchMasterPage.ResultSet = SearchResult.Entities;
            }
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("ResultByBarCode.aspx?Params=" + tbSearch.Text.Trim());
        }

        protected override void SetSearchConditions()
        {
            SearchConditions = string.Format("Barcode = {0}", tbSearch.Text.Trim());
        }
    }
}