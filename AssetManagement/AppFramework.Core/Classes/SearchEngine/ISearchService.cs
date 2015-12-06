﻿using System;
using System.Collections.Generic;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine
{
    public interface ISearchService
    {
        IEnumerable<AppFramework.Core.Classes.Asset> FindByBarcode(string barcode);

        IEnumerable<Entities.IIndexEntity> FindByKeywords(
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
            long assetId = 0);

        List<SearchCounter> GetCounters(
            Guid searchId, 
            long userId,
            string keywords, 
            string configsIds, 
            string taxonomyItemsIds, 
            TimePeriodForSearch time, 
            bool type);

        SearchTracking GetTrackingBySearchId(Guid searchId, long userId);

        IEnumerable<Entities.IIndexEntity> GetSearchResultsBySearchId(Guid searchId, long userId);

        /// <summary>
        /// Find by Type
        /// </summary>
        /// <returns></returns>
        List<IIndexEntity> FindByType(Guid searchId, long userId, long assetTypeUid, List<AttributeElement> elements, string configsIds = "", string taxonomyItemsIds = "", TimePeriodForSearch time = TimePeriodForSearch.CurrentTime, Entities.Enumerations.SearchOrder order = Entities.Enumerations.SearchOrder.Relevance, int pageNumber = 1, int pageSize = 20, bool enableTracking = true);

        void SaveSearchQuery(SearchQuery searchQuery);
        SearchQuery GetSearchQuery(Guid searchId);
    }
}
