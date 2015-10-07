using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppFramework.Entities;
using AppFramework.Core.AC.Authentication;
using AppFramework.DataProxy;
using LinqKit;

namespace AppFramework.Core.Classes
{
    public class ReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

        public ReservationService(IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            if (authenticationService == null)
                throw new ArgumentNullException("IAuthenticationService");

            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Check is current asset is reserved now
        /// </summary>
        /// <returns>Reservation or null</returns>
        public Reservation IsAssetBorrowed(Asset asset)
        {
            var reservations = GetOpenReservations(asset.GetConfiguration().UID, asset.UID);
            return reservations.SingleOrDefault(r => 
                r.StartDate <= DateTime.Today && r.EndDate >= DateTime.Today && r.IsBorrowed);
        }

        /// <summary>
        /// Check if this asset is reserved for future use or currently borrowed
        /// </summary>
        /// <returns>Reservation or null</returns>
        public Reservation IsAssetReserved(Asset asset)
        {
            var reservations = GetOpenReservations(asset.GetConfiguration().UID, asset.UID);
            return reservations.SingleOrDefault(r => r.StartDate <= DateTime.Today && r.EndDate >= DateTime.Today);
        }

        public Reservation Reserve(long reservationUid, DateTime startDate, DateTime endDate,
            long assetTypeUid, long assetUid, long? borrowerId, string reason)
        {
            if (IsOverlaps(reservationUid, startDate, endDate, assetTypeUid, assetUid))
                throw new ReservationException("Reservation conflict found. " +
                                               "It can be caused by existing reservation within the same period of time. " +
                                               "Please check/rearrange existing reservations first.");

            var reservation = reservationUid > 0
                ? _unitOfWork.ReservationRepository.SingleOrDefault(r => r.ReservationUID == reservationUid)
                : new Reservation();

            reservation.StartDate = startDate;
            reservation.EndDate = endDate;
            reservation.DynEntityConfigUID = assetTypeUid;
            reservation.DynEntityUID = assetUid;
            reservation.Borrower = borrowerId.HasValue 
                ? borrowerId.Value 
                : (long)_authenticationService.CurrentUser.ProviderUserKey;
            reservation.IsBorrowed = false;
            reservation.IsClosed = false;
            reservation.IsDamaged = false;
            reservation.Reason = reason;
            reservation.UpdateUserId = (long)_authenticationService.CurrentUser.ProviderUserKey;
            reservation.UpdateDate = DateTime.Now;

            if (reservationUid > 0)
            {
                _unitOfWork.ReservationRepository.Update(reservation);
            }
            else
            {
                reservation.ReservationDate = DateTime.Now;
                _unitOfWork.ReservationRepository.Insert(reservation);
            }
            _unitOfWork.Commit();
            return reservation;
        }

        public Reservation Borrow(long reservationUid)
        {
            var reservation = _unitOfWork.ReservationRepository
                .SingleOrDefault(r => r.ReservationUID == reservationUid);
            reservation.IsBorrowed = true;
            reservation.UpdateDate = DateTime.Now;
            reservation.UpdateUserId = (long)_authenticationService.CurrentUser.ProviderUserKey;

            _unitOfWork.ReservationRepository.Update(reservation);
            _unitOfWork.Commit();
            return reservation;
        }

        public Reservation ReleaseBorrowed(long reservationUid, bool isDamaged, string remarks)
        {
            var reservation = _unitOfWork.ReservationRepository
                .SingleOrDefault(r => r.ReservationUID == reservationUid);
            reservation.IsBorrowed = false;
            reservation.IsClosed = true;
            reservation.Remarks = remarks;
            reservation.IsDamaged = isDamaged;
            reservation.UpdateDate = DateTime.Now;
            reservation.UpdateUserId = (long)_authenticationService.CurrentUser.ProviderUserKey;

            _unitOfWork.ReservationRepository.Update(reservation);
            _unitOfWork.Commit();
            return reservation;
        }

        public void CancelReservation(long reservationUid)
        {
            var reservation = _unitOfWork.ReservationRepository
                .SingleOrDefault(r => r.ReservationUID == reservationUid);

            reservation.IsClosed = true;
            reservation.UpdateDate = DateTime.Now;
            reservation.UpdateUserId = (long)_authenticationService.CurrentUser.ProviderUserKey;

            _unitOfWork.ReservationRepository.Update(reservation);
            _unitOfWork.Commit();
        }

        private bool IsOverlaps(long reservationUid, DateTime startDate, DateTime endDate, long assetTypeUid,
            long assetUid)
        {
            var reservs = GetOpenReservations(assetTypeUid, assetUid);
            //Neither StartDate nor EndDate can be within any already existing reservation range
            //Intervals must be distinct
            return
                reservs.Any(
                    r => r.ReservationUID != reservationUid && ((startDate >= r.StartDate && startDate <= r.EndDate) ||
                                                                (endDate >= r.StartDate && endDate <= r.EndDate) ||
                                                                (startDate <= r.StartDate && endDate >= r.EndDate)));
        }

        public IEnumerable<Reservation> GetOpenReservations(long assetTypeUid, long assetUid)
        {
            var predicate = GetInitialPredicate(assetTypeUid);
            predicate = predicate.And(p => p.DynEntityUID == assetUid);
            predicate = predicate.And(p => p.IsClosed == false);
            return _unitOfWork.ReservationRepository.Get(predicate, rs => rs.OrderBy(r => r.StartDate));
        }

        public IEnumerable<Reservation> GetReservations(long assetTypeUid, bool activeReservationsOnly)
        {
            var predicate = GetInitialPredicate(assetTypeUid);
            if (activeReservationsOnly)
                predicate = predicate.And(p => p.IsClosed == false);
            return _unitOfWork.ReservationRepository
                .Get(predicate, rs => rs.OrderBy(r => r.StartDate));
        }

        private Expression<Func<Reservation, bool>> GetInitialPredicate(long assetTypeUid)
        {
            var predicate = PredicateBuilder.True<Reservation>();
            predicate = predicate.And(p => p.DynEntityConfigUID == assetTypeUid);
            if (!_authenticationService.CurrentUser.IsAdministrator)
            {
                var currentUser = _authenticationService.CurrentUserId;
                predicate = predicate.And(p => p.UpdateUserId == currentUser);
            }
            return predicate;
        }
    }
}
