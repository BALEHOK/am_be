using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Exceptions;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using AssetManager.WebApi.Extensions;
using AssetManager.WebApi.Models.Search;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private readonly ISearchService _searchService;
        private readonly ISearchResultMapper _searchResultMapper;
        private readonly IAdvanceSearchModelMapper _advanceSearchModelMapper;
        private readonly IAdvanceSearchModelSearchQueryMapper _advanceSearchModelSearchQueryMapper;

        public SearchController(
            ISearchService searchService,
            IAdvanceSearchModelMapper advanceSearchModelMapper,
            IAdvanceSearchModelSearchQueryMapper advanceSearchModelSearchQueryMapper, ISearchResultMapper searchResultMapper)
        {
            if (searchService == null)
                throw new ArgumentNullException("searchService");
            _searchService = searchService;

            if (advanceSearchModelMapper == null)
                throw new ArgumentNullException("advanceSearchModelMapper");
            _advanceSearchModelMapper = advanceSearchModelMapper;

            if (advanceSearchModelSearchQueryMapper == null)
                throw new ArgumentNullException("advanceSearchModelSearchQueryMapper");
            _advanceSearchModelSearchQueryMapper = advanceSearchModelSearchQueryMapper;
            if (searchResultMapper == null)
                throw new ArgumentNullException("searchResultMapper");
            _searchResultMapper = searchResultMapper;
        }

        /// <summary>
        /// Performs search by query
        /// </summary>
        [Route(""), HttpGet]
        [CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 120)]
        public SearchResultModel BySimpleQuery([FromUri]SimpleSearchModel model)
        {
            if (!ValidateAndUpdateSimpleSearchModel(model))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            Debug.Assert(model.SearchId != null, "model.SearchId == null");

            var userId = User.GetId();
            
            var result = _searchService.FindByKeywords(
                model.Query,
                model.SearchId.Value,
                userId,
                pageNumber: model.Page,
                configsIds: model.AssetType.HasValue ? model.AssetType.ToString() : string.Empty,
                taxonomyItemsIds: model.Taxonomy.HasValue ? model.Taxonomy.ToString() : string.Empty,
                order: model.SortBy.HasValue
                    ? (Enumerations.SearchOrder)model.SortBy.Value
                    : Enumerations.SearchOrder.Relevance,
                time: model.Context == 1
                    ? TimePeriodForSearch.CurrentTime
                    : TimePeriodForSearch.History,
                    attributeId: model.AttributeId.GetValueOrDefault(),
                    assetId: model.AssetId.GetValueOrDefault()
            );

            return _searchResultMapper.GetSearchResultModel(model.SearchId.Value, result);
        }

        private static bool ValidateAndUpdateSimpleSearchModel(SimpleSearchModel model)
        {
            EnsureSearchId(model);

            if (model.AttributeId.HasValue || model.AssetId.HasValue)
            {
                return model.AttributeId.HasValue && model.AssetId.HasValue && model.AssetType.HasValue;
            }

            return true;
        }


        /// <summary>
        /// Performs search by type
        /// </summary>
        [Route("bytype"), HttpPost]
        [CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 120)]
        public SearchResultModel ByType(AdvanceSearchModel model)
        {
            EnsureSearchId(model);
            Debug.Assert(model.SearchId != null, "model.SearchId == null");

            var userId = User.GetId();
            var attributeElements = _advanceSearchModelMapper.GetAttributeElements(model);
            var result = _searchService.FindByType(
                model.SearchId.Value,
                userId,
                model.AssetType.Id,
                attributeElements,
                time: model.AssetTypeContext,
                order: model.SortBy,
                pageNumber: model.Page);

			return _searchResultMapper.GetSearchResultModel(model.SearchId.Value, result);
        }

        [Route("bytype/model"), HttpPost]
        public string PutSearchByTypeModel(AdvanceSearchModel model)
        {
            EnsureSearchId(model);

            var searchQuery = _advanceSearchModelSearchQueryMapper.GetSearchQuery(model);

            _searchService.SaveSearchQuery(searchQuery);

            return model.SearchId.ToString();
        }

        [Route("bytype/model"), HttpGet]
        public AdvanceSearchModel GetSearchByTypeModel(Guid searchId)
        {
            var searchQuery = _searchService.GetSearchQuery(searchId);
            if (searchQuery == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return _advanceSearchModelSearchQueryMapper.GetAdvanceSearchModel(searchQuery);
        }

        /// <summary>
        /// Returns search results statistics by given search id
        /// </summary>
        /// <param name="searchId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("counters"), HttpGet]
        [CacheOutput(ClientTimeSpan = 30, ServerTimeSpan = 30)]
        public SearchCountersModel GetCounters(Guid searchId, string query = null)
        {
            var userId = User.GetId();

            // if no query => getting counters for search by type
            var result = _searchService.GetCounters(
                searchId, userId, query, "", "", TimePeriodForSearch.CurrentTime, string.IsNullOrEmpty(query));

            if (result.Any())
            {
                return new SearchCountersModel
                {
                    TotalCount = result.Single(e => e.Type == "TotalCount").Count.Value,
                    AssetTypes = result.Where(e => e.Type == "AssetType")
                        .Select(e => new SearchCounterModel
                        {
                            Id = e.id.Value,
                            Count = e.Count.Value,
                            Name = e.Name
                        })
                        .ToList(),
                    Taxonomies = result.Where(e => e.Type == "Taxonomy")
                        .Select(e => new SearchCounterModel
                        {
                            Id = e.id.Value,
                            Count = e.Count.Value,
                            Name = e.Name
                        })
                        .ToList()
                };
            }
            return new SearchCountersModel();
        }

        /// <summary>
        /// Returns tracking information on particular search request by its id
        /// </summary>
        /// <param name="searchId"></param>
        /// <returns></returns>
        [Route("tracking"), HttpGet]
        public SearchTrackingModel GetTrackingInfo(Guid searchId)
        {
            var userId = User.GetId();
            var tracking = _searchService.GetTrackingBySearchId(searchId, userId);
            if (tracking == null)
                throw new EntityNotFoundException(
                    "SearchTracking with given searchId doesn't exists or not accessible.");
            return new SearchTrackingModel
            {
                SearchType = (SearchType) tracking.SearchType,
                VerboseString = tracking.VerboseString
            };
        }

        private static void EnsureSearchId(SearchModelBase model)
        {
            if (!model.SearchId.HasValue || model.SearchId == Guid.Empty)
            {
                model.SearchId = Guid.NewGuid();
            }
        }
    }
}