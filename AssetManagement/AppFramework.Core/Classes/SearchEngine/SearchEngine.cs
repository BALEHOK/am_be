using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.Presentation;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine
{
    /// <summary>
    /// Facade of Search system
    /// </summary>
    public class SearchEngine : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaxonomyItemService _taxonomyItemService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly ISearchTracker _searchTracker;
        private readonly ITypeSearch _typeSearch;

        public SearchEngine(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            ITaxonomyItemService taxonomyItemService,
            ISearchTracker searchTracker,
            ITypeSearch typeSearch)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (taxonomyItemService == null)
                throw new ArgumentNullException("taxonomyItemService");
            _taxonomyItemService = taxonomyItemService;
            if (searchTracker == null)
                throw new ArgumentNullException("searchTracker");
            _searchTracker = searchTracker;
            if (typeSearch == null)
                throw new ArgumentNullException("typeSearch");
            _typeSearch = typeSearch;
        }

        public IEnumerable<Asset> FindByBarcode(string barcode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search by keywords, taxonomies
        /// </summary>
        public IEnumerable<IIndexEntity> FindByKeywords(
            string querystring,
            Guid searchId,
            long userId,
            string configsIds = "",
            string taxonomyItemsIds = "",
            TimePeriodForSearch time = TimePeriodForSearch.CurrentTime,
            Entities.Enumerations.SearchOrder order = Entities.Enumerations.SearchOrder.Relevance,
            int pageNumber = 1,
            int pageSize = 20,
            bool enableTracking = true,
            long attributeId = 0,
            long assetId = 0)
        {
            if (enableTracking)
            {
                _searchTracker.LogSearchByKeywordsRequest(
                    searchId,
                    userId,
                    new SearchParameters
                    {
                        QueryString = querystring,
                        ConfigsIds = configsIds,
                        TaxonomyItemsIds = taxonomyItemsIds,
                        Time = time,
                        Order = order
                    }, querystring);
            }

            var rdResults = _unitOfWork.SqlProvider.ExecuteReader(
                "_cust_SearchByKeywords",
                new[]
                {
                    new SqlParameter("SearchId", searchId),
                    new SqlParameter("UserId", userId),
                    new SqlParameter("keywords", querystring),
                    new SqlParameter("ConfigIds",
                        configsIds != null ? string.Join(" ", configsIds.Split(',')) : null),
                    new SqlParameter("taxonomyItemsIds",
                        taxonomyItemsIds != null ? string.Join(" ", taxonomyItemsIds.Split(',')) : null),
                    new SqlParameter("active", time == TimePeriodForSearch.CurrentTime),
                    new SqlParameter("orderby", (byte) order),
                    new SqlParameter("PageNumber", pageNumber),
                    new SqlParameter("PageSize", pageSize),
                    new SqlParameter("attributeId", attributeId),
                    new SqlParameter("assetId", assetId)
                },
                CommandType.StoredProcedure);

            var entities = new List<IIndexEntity>();
            var typesInSearchLocalCache = new Dictionary<long, AssetType>();
            var helper = new Helper(_unitOfWork);
            while (rdResults.Read())
            {
                var entity = new f_cust_SearchByKeywords_Result
                {
                    IndexUid = rdResults.GetInt64(0),
                    DynEntityUid = rdResults.GetInt64(1)
                };
                if (rdResults[2] != DBNull.Value)
                    entity.BarCode = rdResults.GetString(2);
                if (rdResults[3] != DBNull.Value)
                    entity.Name = rdResults.GetString(3);
                if (rdResults[4] != DBNull.Value)
                    entity.Description = rdResults.GetString(4);
                if (rdResults[5] != DBNull.Value)
                    entity.Keywords = rdResults.GetString(5);
                if (rdResults[6] != DBNull.Value)
                    entity.EntityConfigKeywords = rdResults.GetString(6);
                if (rdResults[7] != DBNull.Value)
                    entity.AllAttrib2IndexValues = rdResults.GetString(7);
                if (rdResults[8] != DBNull.Value)
                    entity.AllContextAttribValues = rdResults.GetString(8);
                if (rdResults[9] != DBNull.Value)
                    entity.AllAttribValues = rdResults.GetString(9);
                if (rdResults[10] != DBNull.Value)
                    entity.CategoryKeywords = rdResults.GetString(10);
                if (rdResults[11] != DBNull.Value)
                    entity.TaxonomyKeywords = rdResults.GetString(11);
                if (rdResults[12] != DBNull.Value)
                    entity.User = rdResults.GetString(12);
                if (rdResults[13] != DBNull.Value)
                    entity.LocationUid = rdResults.GetInt64(13);
                if (rdResults[14] != DBNull.Value)
                    entity.Location = rdResults.GetString(14);
                if (rdResults[15] != DBNull.Value)
                    entity.Department = rdResults.GetString(15);
                if (rdResults[16] != DBNull.Value)
                    entity.DynEntityConfigUid = rdResults.GetInt64(16);
                if (rdResults[17] != DBNull.Value)
                    entity.UpdateDate = rdResults.GetDateTime(17);
                if (rdResults[18] != DBNull.Value)
                    entity.CategoryUids = rdResults.GetString(18);
                if (rdResults[19] != DBNull.Value)
                    entity.TaxonomyUids = rdResults.GetString(19);
                if (rdResults[20] != DBNull.Value)
                    entity.OwnerId = rdResults.GetInt64(20);
                if (rdResults[21] != DBNull.Value)
                    entity.DepartmentId = rdResults.GetInt64(21);
                if (rdResults[22] != DBNull.Value)
                    entity.DynEntityId = rdResults.GetInt64(22);
                if (rdResults[23] != DBNull.Value)
                    entity.TaxonomyItemsIds = rdResults.GetString(23);
                if (rdResults[24] != DBNull.Value)
                    entity.DynEntityConfigId = rdResults.GetInt64(24);
                if (rdResults[25] != DBNull.Value)
                    entity.DisplayValues = rdResults.GetString(25);
                if (rdResults[26] != DBNull.Value)
                    entity.DisplayExtValues = rdResults.GetString(26);
                if (rdResults[27] != DBNull.Value)
                    entity.UserId = rdResults.GetInt64(27);
                entity.rownumber = rdResults.GetInt32(28);

                if (!typesInSearchLocalCache.ContainsKey(entity.DynEntityConfigId))
                    typesInSearchLocalCache.Add(entity.DynEntityConfigId,
                        _assetTypeRepository.GetById(entity.DynEntityConfigId));
                entity.Subtitle = helper.BuildSubtitle(entity,
                    typesInSearchLocalCache[entity.DynEntityConfigId]);

                entities.Add(entity);
            }
            rdResults.Close();
            return entities;
        }

        /// <summary>
        /// Find by Type
        /// </summary>
        /// <returns></returns>
        public List<IIndexEntity> FindByType(Guid searchId, long userId, long assetTypeId, List<AttributeElement> elements, string taxonomyItemsIds = "", TimePeriodForSearch time = TimePeriodForSearch.CurrentTime, Entities.Enumerations.SearchOrder order = Entities.Enumerations.SearchOrder.Relevance, int pageNumber = 1, int pageSize = 20, bool enableTracking = true)
        {
            if (enableTracking)
            {
                _searchTracker.LogSearchByTypeRequest(
                    searchId,
                    userId,
                    new SearchParameters
                    {
                        Elements = elements,
                        ConfigsIds = assetTypeId.ToString(),
                        TaxonomyItemsIds = taxonomyItemsIds,
                        Time = time,
                        Order = order
                    });
            }
            
            var result = _typeSearch.FindByType(searchId, userId, assetTypeId, elements,
                taxonomyItemsIds, time, order, pageNumber, pageSize);
            return result;
        }

        public void SaveSearchQuery(SearchQuery searchQuery)
        {
            var query = _unitOfWork.SearchQueryRepository.SingleOrDefault(q => q.SearchId == searchQuery.SearchId);
            if (query != null)
            {
                _unitOfWork.SearchQueryRepository.Delete(query);
            }
            _unitOfWork.SearchQueryRepository.Insert(searchQuery);
        }

        public SearchQuery GetSearchQuery(Guid searchId)
        {
            return _unitOfWork.SearchQueryRepository
                .SingleOrDefault(q => q.SearchId == searchId,
                q => q.SearchQueryAttributes.Select(a => a.ComplexValue));
        }

        public static List<KeyValuePair<long, string>> FindIdNameBySearchPattern(IUnitOfWork unitOfWork, long assetTypeId, long assetTypeAttributeId, out int totalCount, string searchPattern = "", int pageNumber = 1, int pageSize = 100)
        {
            var at = AssetType.GetByID(assetTypeId);
            var fieldName = at.Attributes.Single(a => a.ID == assetTypeAttributeId).DBTableFieldName;

            var parameters = new List<SqlParameter>();
            var parameterName = string.Format("@{0}", fieldName);
            parameters.Add(new SqlParameter(parameterName, string.Format("%{0}%", searchPattern)));

            var queryWithPaging = string.Format(@"
                WITH search AS
                (SELECT DISTINCT
                    DynEntityId, 
                    Name, 
                    ROW_NUMBER() OVER(ORDER BY [{1}]) as intRow, 
                    COUNT(*) OVER() AS TotalCount 
                    FROM [{0}]
                    WHERE [{1}] LIKE {2} AND ActiveVersion = 1
                )
                SELECT DynEntityId, Name, TotalCount FROM search
                WHERE intRow BETWEEN @intStartRow AND @intEndRow",
                at.DBTableName,
                fieldName,
                parameterName);

            if (pageNumber < 1)
                pageNumber = 1;
            parameters.Add(new SqlParameter("@intStartRow", (pageNumber - 1)*pageSize));
            parameters.Add(new SqlParameter("@intEndRow", pageNumber*pageSize - 1));

            var reader = unitOfWork.SqlProvider.ExecuteReader(
                queryWithPaging,
                parameters.ToArray());

            var result = new List<KeyValuePair<long, string>>();
            totalCount = 0;
            while (reader.Read())
            {
                result.Add(new KeyValuePair<long, string>(reader.GetInt64(0), reader.GetString(1)));
                
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(2);
                }
            }
            reader.Close();
            return result;
        }

        public List<SearchCounter> GetCounters(
            Guid searchId,
            long userId,
            string keywords,
            string configsIds,
            string taxonomyItemsIds,
            TimePeriodForSearch time,
            bool type)
        {
            var counters = new List<SearchCounter>();
            var rdCounters = _unitOfWork.SqlProvider.ExecuteReader(
                "_cust_GetSrchCount",
                new[]
                {
                    new SqlParameter("SearchId", searchId),
                    new SqlParameter("UserId", userId),
                    new SqlParameter("keywords", keywords),
                    new SqlParameter("ConfigIds", configsIds != null ? string.Join(" ", configsIds.Split(',')) : null),
                    new SqlParameter("taxonomyItemsIds",
                        taxonomyItemsIds != null ? string.Join(" ", taxonomyItemsIds.Split(',')) : null),
                    new SqlParameter("active", time == TimePeriodForSearch.CurrentTime),
                    new SqlParameter("type", type)
                },
                CommandType.StoredProcedure);

            while (rdCounters.Read())
            {
                // 0        1       2       3   4
                // SearchId	UserId	Type	id	Count
                var counter = new SearchCounter
                {
                    id = rdCounters.GetInt64(3),
                    Type = rdCounters.GetString(2),
                    Count = rdCounters.GetInt32(4)
                };

                if (counter.Type == "AssetType")
                    counter.Name = _assetTypeRepository.GetById(counter.id.Value).Name;
                else if (counter.Type == "Taxonomy")
                    counter.Name = _taxonomyItemService.GetActiveItemById(counter.id.Value).Name;
                counters.Add(counter);
            }
            rdCounters.Close();
            return counters;
        }

        public IEnumerable<IIndexEntity> GetSearchResultsByTracking(SearchTracking searchTracking, long userId)
        {
            if (searchTracking == null)
                throw new EntityNotFoundException("Cannot find search request parameters by given SearchId");

            var parameters = SearchParameters.GetFromXml(searchTracking.Parameters);
            var searchType = (SearchType) searchTracking.SearchType;

            switch (searchType)
            {
                case SearchType.SearchByType:
                {
                    return FindByType(
                        searchTracking.SearchId,
                        userId,
                        long.Parse(parameters.ConfigsIds),
                        parameters.Elements,
                        parameters.TaxonomyItemsIds,
                        parameters.Time,
                        parameters.Order, 1, int.MaxValue, false);
                }
                case SearchType.SearchByKeywords:
                {
                    return FindByKeywords(
                        parameters.QueryString,
                        searchTracking.SearchId,
                        userId,
                        parameters.ConfigsIds,
                        parameters.TaxonomyItemsIds,
                        parameters.Time,
                        parameters.Order, 1, int.MaxValue, false);
                }
            }

            return new List<IIndexEntity>();
        }
    }
}