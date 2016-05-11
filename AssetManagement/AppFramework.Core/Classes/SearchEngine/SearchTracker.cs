using AppFramework.Core.Exceptions;

namespace AppFramework.Core.Classes.SearchEngine
{
    using System;
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.Classes.SearchEngine.Enumerations;
    using AppFramework.Entities;
    using AppFramework.DataProxy;

    /// <summary>
    /// Tracks all search requests
    /// </summary>
    public class SearchTracker : AppFramework.Core.Classes.SearchEngine.ISearchTracker
    {
        private readonly IUnitOfWork _unitOfWork;
        public SearchTracker(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Logs simple search action
        /// </summary>
        public void LogSearchByKeywordsRequest(Guid searchId, long userId, SearchParameters parameters, string verboseString)
        {
            EnsuretrackingNotExists(searchId);

            _unitOfWork.SearchTrackingRepository.Insert(new SearchTracking
            {
                SearchId = searchId,
                SearchType = (short)SearchType.SearchByKeywords,
                Parameters = parameters.ToXml(),
                UpdateUser = userId,
                UpdateDate = DateTime.Now,
                VerboseString = verboseString
            });
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Logs search bby type action
        /// </summary>
        public void LogSearchByTypeRequest(Guid searchId, long userId, SearchParameters parameters)
        {
            EnsuretrackingNotExists(searchId);

            _unitOfWork.SearchTrackingRepository.Insert(new SearchTracking
            {
                SearchId = searchId,
                SearchType = (short)SearchType.SearchByType,
                Parameters = parameters.ToXml(),
                UpdateUser = userId,
                UpdateDate = DateTime.Now,
                VerboseString = string.Empty
            });
            _unitOfWork.Commit();
        }


        /// <summary>
        /// Returns tracked search action by its id
        /// </summary>
        /// <param name="trackingId"></param>
        /// <returns></returns>
        [Obsolete("Use GetTrackingBySearchIdUserId instead for more security")]
        public SearchTracking GetTrackingById(long trackingId)
        {
            return _unitOfWork.SearchTrackingRepository
                .SingleOrDefault(s => s.Id == trackingId);
        }

        /// <summary>
        /// Returns tracked search action by its id and user id
        /// </summary>
        public SearchTracking GetTrackingBySearchIdUserId(Guid searchId, long userId)
        {
            return _unitOfWork.SearchTrackingRepository
                .SingleOrDefault(s => s.SearchId == searchId && s.UpdateUser == userId);
        }

        private void EnsuretrackingNotExists(Guid searchId)
        {
            var tracking = _unitOfWork.SearchTrackingRepository
                .SingleOrDefault(t => t.SearchId == searchId);
            // don't allow multiple trackings with same search id
            if (tracking != null)
                _unitOfWork.SearchTrackingRepository.Delete(tracking);
        }
    }
}
