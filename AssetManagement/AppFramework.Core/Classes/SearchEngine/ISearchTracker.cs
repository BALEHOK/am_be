using System;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine
{
    public interface ISearchTracker
    {
        [Obsolete("Use GetTrackingBySearchIdUserId instead for more security")]
        SearchTracking GetTrackingById(long trackingId);

        SearchTracking GetTrackingBySearchIdUserId(Guid searchId, long userId);
        void LogSearchByKeywordsRequest(Guid searchId, long userId, SearchParameters parameters, string verboseString);
        void LogSearchByTypeRequest(Guid searchId, long userId, SearchParameters parameters);
    }
}