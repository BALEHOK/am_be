using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Linq;

namespace AssetSite.Search
{
    public partial class NewResultByCategory : SearchResultPage
    {
        [Dependency]
        public ISearchService SearchService { get; set; }

        protected override void ProcessRequest()
        {
            if (!IsPostBack && !string.IsNullOrEmpty(TaxonomyItemsIds))
            {
                if (Session["TaskConfig"] != null)
                {
                    var descriptor = new SearchConfigurationDescriptor
                        {
                            SearchType = SearchType.SearchByCategory,
                            ConfigsIds = this.ConfigsIds,
                            TaxonomyItemsIds = this.TaxonomyItemsIds,
                            Params = this.Params,
                            Time = this.Period
                        };
                    Session["TaskData"] = descriptor.Serialize();
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
            }
            else if (string.IsNullOrEmpty(TaxonomyItemsIds))
            {
                SearchMasterPage.ResultSet[SearchId] = new List<IIndexEntity>();
                Response.Redirect("/Search/SearchByCategory.aspx");
            }
        }

        protected override void SetSearchConditions()
        {
            if (Request.QueryString["TaxItems"] != null)
            {
                SearchConditions[SearchId] = Request.QueryString["TaxItems"].ToString();
                if (!string.IsNullOrEmpty(Params))
                    SearchConditions[SearchId] += string.Format(" ({0})", Params);
            }
        }
    }
}