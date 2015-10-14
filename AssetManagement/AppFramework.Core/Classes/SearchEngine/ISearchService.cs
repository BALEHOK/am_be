using System;
using System.Collections.Generic;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine
{
    public interface ISearchService
    {
        int NewSearchId();

        IEnumerable<AppFramework.Core.Classes.Asset> FindByBarcode(string barcode);

        IEnumerable<Entities.IIndexEntity> FindByKeywords(
            string querystring, 
            long searchId, 
            long userId,
            string configsIds = "", 
            string taxonomyItemsIds = "", 
            TimePeriodForSearch time = TimePeriodForSearch.CurrentTime, 
            Entities.Enumerations.SearchOrder order = Entities.Enumerations.SearchOrder.Relevance, 
            int pageNumber = 1, 
            int pageSize = 20,
            bool enableTracking = true);

        List<SearchCounter> GetCounters(
            long searchId, 
            long userId,
            string keywords, 
            string configsIds, 
            string taxonomyItemsIds, 
            TimePeriodForSearch time, 
            bool type);
        
        SearchTracking GetTrackingBySearchId(long searchId, long userId);

        IEnumerable<Entities.IIndexEntity> GetSearchResultsBySearchId(long searchId, long userId);

        /// <summary>
        /// Find by Type
        /// </summary>
        /// <returns></returns>
        List<IIndexEntity> FindByTypeContext(
            long searchId,
            long userId,
            long? assetTypeUid,
            IEnumerable<AttributeElement> elements,
            string configsIds = "",
            string taxonomyItemsIds = "",
            TimePeriodForSearch time = TimePeriodForSearch.CurrentTime,
            Entities.Enumerations.SearchOrder order = Entities.Enumerations.SearchOrder.Relevance,
            int pageNumber = 1,
            int pageSize = 20);
    }
}
