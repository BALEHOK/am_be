using System;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;
using AssetSite.Helpers;
using AssetSite.MasterPages;
using System.Collections.Generic;
using AssetManager.Infrastructure;

namespace AssetSite.Search
{
    public abstract class SearchResultPage : BasePage
    {
        public int PageNumber { get { return _pageNumber; } }
        public int PageSize { get { return _pageSize; } }
        public Enumerations.SearchOrder OrderBy { get { return _order; } }
        public SearchOutput SearchResult { get; set; }
        public int SearchId { get { return _searchId; } }
        public string ConfigsIds { get; set; }
        public string TaxonomyItemsIds { get; set; }
        public string Params { get; set; }
        public TimePeriodForSearch Period { get; set; }

        private int _pageNumber = 1;
        private int _pageSize = ApplicationSettings.RecordsPerPage;
        private Enumerations.SearchOrder _order = Enumerations.SearchOrder.Relevance;
        private int _searchId;
        private IEnvironmentSettings _env = new EnvironmentSettings();

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (Request[_env.PageNumber] != null)
                int.TryParse(Request[_env.PageNumber].ToString(), out _pageNumber);

            if (Request[_env.PageSize] != null)
                int.TryParse(Request[_env.PageSize].ToString(), out _pageSize);

            int orderid;
            if (Request["OrderBy"] != null && int.TryParse(Request["OrderBy"].ToString(), out orderid))
                _order = (Enumerations.SearchOrder)orderid;

            if (Request["SearchId"] != null)
            {
                int.TryParse(Request["SearchId"].ToString(), out _searchId);
            }
            else if (!string.IsNullOrEmpty(Request.Url.Query))
            {
                var url = Request.Url.OriginalString + "&SearchId=" + _searchId;
                Response.Redirect(url);
            }

            ConfigsIds = Request["ConfigsIds"];
            TaxonomyItemsIds = Request["TaxonomyItemsIds"];

            Params = string.Empty;
            if (Request.QueryString["Params"] != null)
            {
                Params = QueryFormatter.Format(Request.QueryString["Params"].Trim());
            }

            int period = (int)TimePeriodForSearch.CurrentTime;
            int.TryParse(Request.QueryString["Time"], out period);
            Period = (TimePeriodForSearch)period;
        }

        public Dictionary<int, string> SearchConditions
        {
            get
            {
                if (Session["SearchConditions"] == null)
                {
                    Session["SearchConditions"] = new Dictionary<int, string>();
                }
                return Session["SearchConditions"] as Dictionary<int, string>;
            }
        }

        protected MasterPageSearchResult SearchMasterPage
        {
            get
            {
                return Master as MasterPageSearchResult;
            }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            ProcessRequest();
            SetSearchConditions();
        }

        protected abstract void ProcessRequest();

        protected abstract void SetSearchConditions();        
    }
}