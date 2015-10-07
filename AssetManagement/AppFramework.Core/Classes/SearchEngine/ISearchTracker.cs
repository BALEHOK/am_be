using AppFramework.Core.Classes.SearchEngine.Enumerations;
using System;
namespace AppFramework.Core.Classes.SearchEngine
{
    public interface ISearchTracker
    {
        AppFramework.Entities.SearchTracking GetTrackingById(long trackingId);
        AppFramework.Entities.SearchTracking GetTrackingBySearchIdUserId(long searchId, long userId);
        void LogSearchRequest(long searchId, SearchType type, string verboseString, SearchParameters parameters, long userId);
    }
}
