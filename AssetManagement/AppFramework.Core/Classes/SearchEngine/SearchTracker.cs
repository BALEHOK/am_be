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
        /// Logs search action
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <param name="resultCount"></param>
        public void LogSearchRequest(
            long searchId,
            SearchType type, 
            string verboseString, 
            SearchParameters parameters, 
            long userId)
        {
            var tracking = _unitOfWork.SearchTrackingRepository
                .SingleOrDefault(t => t.SearchId == searchId);
            // don't allow multiple trackings with same search id
            if (tracking != null)
                _unitOfWork.SearchTrackingRepository.Delete(tracking);
            _unitOfWork.SearchTrackingRepository.Insert(new Entities.SearchTracking()
            {
                SearchId = searchId,
                SearchType = (short)type,
                Parameters = parameters.ToXml(),
                UpdateUser = userId,
                UpdateDate = DateTime.Now,
                VerboseString = verboseString
            });
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Returns tracked search action by its id
        /// </summary>
        /// <param name="trackingId"></param>
        /// <returns></returns>
        public SearchTracking GetTrackingById(long trackingId)
        {
            return _unitOfWork.SearchTrackingRepository
                .SingleOrDefault(s => s.Id == trackingId);
        }

        /// <summary>
        /// Returns tracked search action by its id
        /// </summary>
        /// <param name="trackingId"></param>
        /// <returns></returns>
        public SearchTracking GetTrackingBySearchIdUserId(long searchId, long userId)
        {
            return _unitOfWork.SearchTrackingRepository
                .SingleOrDefault(s => s.SearchId == searchId && s.UpdateUser == userId);
        }
    }
}
