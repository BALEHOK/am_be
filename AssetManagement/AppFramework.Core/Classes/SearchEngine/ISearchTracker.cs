using AppFramework.Core.Classes.SearchEngine.Enumerations;
using System;
namespace AppFramework.Core.Classes.SearchEngine
{
    public interface ISearchTracker
    {
        AppFramework.Entities.SearchTracking GetTrackingById(long trackingId);
        AppFramework.Entities.SearchTracking GetTrackingBySearchIdUserId(Guid searchId, long userId);
        void LogSearchRequest(Guid searchId, SearchType type, string verboseString, SearchParameters parameters, long userId);
    }
}
