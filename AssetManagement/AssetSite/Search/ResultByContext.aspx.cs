using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.ContextSearchElements;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using AppFramework.Entities;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace AssetSite.Search
{
    public partial class NewResultByContext : SearchResultPage
    {
        protected override void ProcessRequest()
        {
            if (!IsPostBack && !string.IsNullOrEmpty(Params))
            {
                if (Session[Request.QueryString["Params"]] as List<AttributeElement> == null)
                    Response.Redirect("SearchByContext.aspx");
                              
                if (Session["TaskConfig"] != null)
                {
                    var desc = new SearchConfigurationDescriptor
                    {
                        SearchType = SearchType.SearchByContext,
                        Params = (Session[Request.QueryString["Params"]] as List<AttributeElement>),
                        Time = this.Period,
                        ConfigsIds = this.ConfigsIds,
                        TaxonomyItemsIds = this.TaxonomyItemsIds
                    };
                    Session["TaskData"] = desc.Serialize();
                }

                var typeSearch = new TypeSearch(
                    AuthenticationService,
                    UnitOfWork,
                    AssetTypeRepository,
                    AssetsService);

                var result = typeSearch.FindByTypeContext(
                    SearchId,
                    null,
                    (Session[Request.QueryString["Params"]] as List<AttributeElement>),
                    ConfigsIds,
                    TaxonomyItemsIds,
                    Period,
                    OrderBy,
                    PageNumber,
                    PageSize);

                SearchMasterPage.ResultSet[SearchId] = result;
            }
            else if (string.IsNullOrEmpty(Params))
            {
                SearchMasterPage.ResultSet[SearchId] = new List<IIndexEntity>();
                Response.Redirect("SearchByContext.aspx");
            }
        }

        protected override void SetSearchConditions()
        {
            string key = Request.QueryString["Params"];
            if (Session[key] != null)
            {
                SearchConditions[SearchId] = 
                    (Session[key] as List<AttributeElement>).ToVerbalString(new ContextForSearch(UnitOfWork));
            }
        }
    }
}