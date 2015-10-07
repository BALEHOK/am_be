using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Reports;
using AppFramework.Reports.CustomReports;
using AppFramework.Reports.Services;
using Microsoft.Practices.Unity;

namespace AssetSite.Search
{
    public partial class NewResultByType : SearchResultPage
    {
        /// <summary>
        /// 
        /// </summary>
        [Dependency]
        public ISearchService SearchService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Dependency]
        public ICustomReportService<CustomDevExpressReport> ReportService { get; set; }


        private string _strparams = string.Empty;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
//            var userRights = claimsIdentity.FindFirst("UserRights").Value;
//            replaceButtons.Visible = userRights.Contains(SecuredModules.ReplaceFuntion);
        }

        protected override void ProcessRequest()
        {
            if (!IsPostBack && Request.QueryString["TypeUID"] != null)
            {
                long assetTypeUid = long.Parse(Request.QueryString["TypeUID"]);
                if (!string.IsNullOrEmpty(Request.QueryString["Params"]))
                {
                    _strparams = Request.QueryString["Params"];
                }
                else if (Request.QueryString["AttributeUId"] != null && Request.QueryString["AssetId"] != null)
                {
                    long attributeUId = long.Parse(Request.QueryString["AttributeUId"]);
                    var type = new OneType(AssetTypeRepository.GetByUid(assetTypeUid), UnitOfWork, AssetsService, AssetTypeRepository);
                    var assetAttribute = type.AssetAttributes.FirstOrDefault(a => a.Configuration.UID == attributeUId);
                    assetAttribute.ValueAsId = long.Parse(Request.QueryString["AssetId"]);
                    List<AttributeElement> arrayparameters = new List<AttributeElement>();
                    arrayparameters.Add(new AttributeElement
                    {
                        DateType = Enumerators.DataType.Asset,
                        AttributeId = type.AssetAttributes.IndexOf(assetAttribute),
                        AssetAttribute = assetAttribute,
                        OperatorId = -1
                    });
                    Guid _guid = Guid.NewGuid();
                    _strparams = _guid.ToString();
                    Session[_strparams] = arrayparameters;
                    Session["ChildAssetsData"] = string.Format("{0};{1}", Request.QueryString["AssetId"], Request.QueryString["AssetTypeId"]);
                    Response.Redirect("~/Search/ResultByType.aspx?Params=" + _guid + "&TypeUID=" + assetTypeUid + "&Time=" + (int)TimePeriodForSearch.CurrentTime);
                }
                if (Session[_strparams] as List<AttributeElement> == null)
                    Response.Redirect("SearchByType.aspx");

                if (Session["TaskConfig"] != null)
                {
                    var desc = new SearchConfigurationDescriptor
                    {
                        SearchType = SearchType.SearchByType,
                        TypeUID = assetTypeUid,
                        Params = Session[_strparams] as List<AttributeElement>,
                        Time = this.Period,
                        ConfigsIds = this.ConfigsIds,
                        TaxonomyItemsIds = this.TaxonomyItemsIds
                    };
                    Session["TaskData"] = desc.Serialize();
                }

                //                var typeSearch = new TypeSearch(
                //                    AuthenticationService,
                //                    UnitOfWork,
                //                    AssetTypeRepository,
                //                    AssetsService);

                //                var result = typeSearch.FindByTypeContext(
                //                    SearchId,
                //                    assetTypeUid,
                //                    (Session[_strparams] as List<AttributeElement>),
                //                    ConfigsIds,
                //                    TaxonomyItemsIds,
                //                    Period,
                //                    OrderBy,
                //                    PageNumber,
                //                    PageSize);
                
                var searchParams = (List<AttributeElement>)Session[_strparams];
                var queryString = searchParams.First().Text;

                var assetType = AssetTypeRepository.GetByUid(assetTypeUid);
                var searchId = SearchService.NewSearchId();
                var result = SearchService.FindByKeywords(
                    queryString, 
                    searchId,
                    AuthenticationService.CurrentUserId,
                    pageNumber: 1, 
                    configsIds: assetType.ID.ToString()).ToList();

                SearchMasterPage.ResultSet[SearchId] = result;

                var customReports = ReportService.GetReportsByTypeId(assetType.ID);

                if (customReports.Count > 0)
                    ReportsPanel.Visible = true;

                customReports.ForEach(report =>
                {
                    ReportsPanel.Reports.Add(report.Name,
                        new Dictionary<string, object>
                        {
                            {"ReportId", report.Id},
                            {"SearchId", searchId},
                            {"Params", _strparams},
                            {"TypeId", assetType.ID},

                            {"ReportType", (int)ReportType.SearchResultReport},
                            {"ReportLayout", (int)ReportLayout.Default}
                        });
                });

                //                ReportsPanel.Reports.Add("Search Results Report (detailed)", 
                //                    new Dictionary<string, object> { 
                //                        {"SearchId", SearchId},
                //                        {"Params", _strparams},
                //                        {"TypeUID", assetTypeUid},
                //                        {"ReportType", (int)ReportType.SearchResultReport},
                //                        {"ReportLayout", (int)ReportLayout.Default}
                //                    });
                //                ReportsPanel.Reports.Add("Search Results Report (compact)", 
                //                    new Dictionary<string, object> { 
                //                        {"SearchId", SearchId},
                //                        {"Params", _strparams},
                //                        {"TypeUID", assetTypeUid},
                //                        {"ReportType", (int)ReportType.SearchResultReport},
                //                        {"ReportLayout", (int)ReportLayout.Compact}
                //                    });
            }
        }

        protected override void SetSearchConditions()
        {
            string key = _strparams;
            if (Session[key] != null)
            {
                SearchConditions[SearchId] =
                    (Session[key] as IEnumerable<AttributeElement>)
                        .ToVerbalString(
                            AssetTypeRepository.GetByUid(
                                long.Parse(Request.QueryString["TypeUID"])),
                            UnitOfWork,
                            AssetsService,
                            AssetTypeRepository);
            }
        }
    }
}