using System;
using System.Linq;
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
        private readonly IAdvanceSearchModelMapper _advanceSearchModelMapper;

        public SearchController(ISearchService searchService, IAdvanceSearchModelMapper advanceSearchModelMapper)
        {
            if (searchService == null)
                throw new ArgumentNullException("searchService");
            _searchService = searchService;

            if (advanceSearchModelMapper == null)
                throw new ArgumentNullException("advanceSearchModelMapper");
            _advanceSearchModelMapper = advanceSearchModelMapper;
        }

        /// <summary>
        /// Performs search by query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="assetType"></param>
        /// <param name="taxonomy"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        [Route(""), HttpGet]
        [CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 120)]
        public SearchResultModel BySimpleQuery(
            string query,
            int page = 1,
            int context = 1,
            Guid? searchId = null,
            int? assetType = null,
            int? taxonomy = null,
            int? sortBy = null)
        {
            if (!searchId.HasValue)
            {
                searchId = Guid.NewGuid();
            }

            var userId = User.GetId();
            var result = _searchService.FindByKeywords(
                query,
                searchId.Value,
                userId,
                pageNumber: page,
                configsIds: assetType.HasValue ? assetType.ToString() : "",
                taxonomyItemsIds: taxonomy.HasValue ? taxonomy.ToString() : "",
                order: sortBy.HasValue
                    ? (Enumerations.SearchOrder) sortBy.Value
                    : Enumerations.SearchOrder.Relevance,
                time: context == 1
                    ? TimePeriodForSearch.CurrentTime
                    : TimePeriodForSearch.History
                );

            return new SearchResultModel
            {
                SearchId = searchId.Value,
                Entities = result.ToList()
            };
        }


        /// <summary>
        /// Performs search by type
        /// </summary>
        [Route("bytype"), HttpPost]
        [CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 120)]
        public SearchResultModel ByType(AdvanceSearchModel model)
        {
            if (model.SearchId == Guid.Empty)
            {
                model.SearchId = Guid.NewGuid();
            }

            var userId = User.GetId();
            var attributeElements = _advanceSearchModelMapper.GetAttributeElements(model);
            var result = _searchService.FindByType(
                model.SearchId,
                userId,
                model.AssetTypeId,
                attributeElements,
                time: model.Context,
                order: model.SortBy,
                pageNumber: model.Page);

            return new SearchResultModel
            {
                SearchId = model.SearchId,
                Entities = result.ToList()
            };
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
                VerboseString = tracking.VerboseString
            };
        }
    }
}