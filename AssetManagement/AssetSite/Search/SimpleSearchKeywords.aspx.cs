using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetSite.Search
{
    public partial class NewSimpleSearchKeywords : SearchResultPage
    {
        [Dependency]
        public ISearchService SearchService { get; set; }

        protected override void ProcessRequest()
        {
            if (!IsPostBack && !string.IsNullOrWhiteSpace(this.Params))
            {
                tbSearch.Text = this.Params;

                rbActive.SelectedIndex = this.Period == TimePeriodForSearch.CurrentTime ? 0 : 1;
                if (Session["TaskConfig"] != null)
                {
                    var desc = new SearchConfigurationDescriptor
                    {
                        SearchType = SearchType.SearchByKeywords,
                        Params = tbSearch.Text,
                        Time = this.Period,
                        ConfigsIds = this.ConfigsIds,
                        TaxonomyItemsIds = this.TaxonomyItemsIds
                    };
                    Session["TaskData"] = desc.Serialize();
                }

                var result = SearchService.FindByKeywords(
                    this.Params,
                    this.SearchId,
                    AuthenticationService.CurrentUserId,
                    this.ConfigsIds,
                    this.TaxonomyItemsIds,
                    this.Period,
                    this.OrderBy,
                    this.PageNumber,
                    this.PageSize);

                SearchMasterPage.ResultSet[SearchId] = result.ToList();
                
                //   if (Request["FromLog"] == null)
                //{
                //    SearchParameters @params = new SearchParameters();
                //    @params.Add("Params", this.Params);
                //    @params.Add("Time", (int)period);
                //    SearchTracker.Log(SearchType.SearchByKeywords, @params, counters.First().t);
                //   }
            }
            else if (string.IsNullOrWhiteSpace(this.Params))
            {
                SearchMasterPage.ResultSet[SearchId] = new List<IIndexEntity>();
            }
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            TimePeriodForSearch period = rbActive.SelectedValue == "1" ? TimePeriodForSearch.CurrentTime : TimePeriodForSearch.History;

            var query = 
                "?Params=" +
                Server.UrlEncode(tbSearch.Text.Trim()) +
                "&Time=" + (int)period;

            Session[Constants.SearchParameters] = Request.Url.GetLeftPart(UriPartial.Path) + query;

            Response.Redirect("~/Search/SimpleSearchKeywords.aspx" + query);
        }

        protected override void SetSearchConditions()
        {
            SearchConditions[SearchId] = string.Format(tbSearch.Text.Trim());
        }
    }
}