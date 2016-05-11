using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AppFramework.Reservations.Exceptions;
using AppFramework.Reservations.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Transactions;

namespace AppFramework.Reservations.Services
{
    public class ReservationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILog _logger;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;

        private readonly Dictionary<long, AssetType> _assetTypes 
            = new Dictionary<long, AssetType>();
        private readonly Dictionary<Tuple<long, long>, string> _assetNames
            = new Dictionary<Tuple<long, long>, string>();
        private readonly Dictionary<long, Asset> _borrowers
            = new Dictionary<long, Asset>();

        public ReservationsService(
            IUnitOfWork unitOfWork, 
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            ILog logger)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;

            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;

            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;

            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        public IEnumerable<ReservationModel> GetAllReservations()
        {
            var entities = _unitOfWork.ReservationRepository.Get();
            var models = entities
                .Select(e => ReservationModel.FromEntity(e))
                .ToList();

            foreach (var model in models)
            {
                var borrower = GetBorrower(model.BorrowerId);
                model.BorrowerConfigId = borrower.Configuration.ID;
                model.BorrowerName = borrower.Name;
            }

            return models;
        }

        public ReservationModel GetReservationById(long reservationId)
        {
            var entity = _unitOfWork.ReservationRepository
                .SingleOrDefault(x => x.ReservationId == reservationId);
            if (entity == null)
                throw new ReservationNotExist();

            var model =  ReservationModel.FromEntity(entity);
            var borrower = GetBorrower(model.BorrowerId);
            model.BorrowerName = borrower.Name;
            model.BorrowerConfigId = borrower.Configuration.ID;
            return model;
        }

        public void ChangeStatus(long reservationId, short state, string comment, long userId)
        {
            var entity = _unitOfWork.ReservationRepository
                .SingleOrDefault(x => x.ReservationId == reservationId);
            if (entity == null)
                throw new ReservationNotExist();

            entity.State = state;
            entity.Comment = comment;
            entity.UpdateUserId = userId;
            entity.UpdateDate = DateTime.Now;
            _unitOfWork.ReservationRepository.Update(entity);
            _unitOfWork.Commit();

            _logger.InfoFormat("Reservation #{0} status updated by user #{1}. New status: {2}, comment: \n{3}\n",
                entity.ReservationId, userId, state, comment);
        }

        public ReservationModel CreateReservation(ReservationModel model, long userId)
        {
            var entity = ReservationModel.ToEntity(model);
            entity.UpdateUserId = userId;
            entity.UpdateDate = DateTime.Now;

            using (var scope = new TransactionScope())
            {
                _unitOfWork.ReservationRepository.Insert(entity);

                foreach (var ra in model.ReservedAssets)
                {
                    if (IsAssetReservedForDates(ra.AssetId, ra.AssetTypeId, entity.StartDate, entity.EndDate))
                        throw new ReservationCollision(ra.AssetId, ra.AssetTypeId, entity.StartDate,
                            entity.EndDate);

                    _unitOfWork.ReservedAssetsRepository.Insert(
                        new Entities.ReservedAsset
                        {
                            DynEntityId = ra.AssetId,
                            DynEntityConfigId = ra.AssetTypeId,
                            UpdateUserId = userId,
                            UpdateDate = DateTime.Now,
                            ReservationId = entity.ReservationId
                        });
                }
                
                _unitOfWork.Commit();
                scope.Complete();
            }

            _logger.InfoFormat("Reservation #{0} created with {1} assets reserved",
                entity.ReservationId, entity.ReservedAssets.Count);

            return ReservationModel.FromEntity(entity);
        }

        public ReservationModel UpdateReservation(long reservationId, ReservationModel model, long userId)
        {
            var entity = _unitOfWork.ReservationRepository
                .SingleOrDefault(x => 
                    x.ReservationId == reservationId, 
                    i => i.ReservedAssets);

            if (entity == null)
                throw new ReservationNotExist();

            entity = ReservationModel.UpdateEntityFromModel(entity, model);
            entity.UpdateUserId = userId;
            entity.UpdateDate = DateTime.Now;

            using (var scope = new TransactionScope())
            {
                _unitOfWork.ReservationRepository.Update(entity);

                for (int i = entity.ReservedAssets.Count - 1; i >= 0; i--)
                {
                    _unitOfWork.ReservedAssetsRepository.Delete(entity.ReservedAssets[i]);
                }

                foreach (var ra in model.ReservedAssets)
                {
                    if (IsAssetReservedForDates(ra.AssetId, ra.AssetTypeId, entity.StartDate, entity.EndDate))
                        throw new ReservationCollision(ra.AssetId, ra.AssetTypeId, entity.StartDate, 
                            entity.EndDate);

                    _unitOfWork.ReservedAssetsRepository.Insert(
                        new Entities.ReservedAsset
                        {
                            ReservationId = reservationId,
                            DynEntityId = ra.AssetId,
                            DynEntityConfigId = ra.AssetTypeId,
                            UpdateUserId = userId,
                            UpdateDate = DateTime.Now,
                        });
                }

                _unitOfWork.Commit();
                scope.Complete();
            }
            
            _logger.InfoFormat("Reservation #{0} updated",
                entity.ReservationId);

            return ReservationModel.FromEntity(entity);
        }

        public IEnumerable<ReservedAssetModel> GetReservationAssets(long reservationId)
        {
            var entity = _unitOfWork.ReservationRepository
                .SingleOrDefault(x => x.ReservationId == reservationId, i => i.ReservedAssets);
            if (entity == null)
                throw new ReservationNotExist();

            return entity.ReservedAssets
                .Select(x => new ReservedAssetModel
                {
                    AssetId = x.DynEntityId,
                    AssetTypeId = x.DynEntityConfigId,
                    AssetTypeName = GetAssetType(x.DynEntityConfigId).Name,
                    AssetName = GetAssetName(x.DynEntityId, x.DynEntityConfigId)
                });
        }

        public IEnumerable<ReservationModel> GetReservationsByAssetTypeIdInDateRange(
            long assetTypeId, DateTime startDate, DateTime endDate)
        {
            var reservations = _unitOfWork.ReservationRepository
                .Where(
                    x => ((EntityFunctions.TruncateTime(x.StartDate) <= startDate.Date &&
                          EntityFunctions.TruncateTime(x.EndDate) > startDate.Date)

                      || (EntityFunctions.TruncateTime(x.StartDate) >= startDate.Date &&
                          EntityFunctions.TruncateTime(x.StartDate) < endDate.Date))

                        && x.State == (short)ReservationState.Open
                        && x.ReservedAssets.Any(a => a.DynEntityConfigId == assetTypeId),
                    inc => inc.ReservedAssets);

            return reservations.Select(entity => 
            {
                var model = ReservationModel.FromEntity(entity);
                model.ReservedAssets = entity.ReservedAssets
                    .Select(x => new ReservedAssetModel
                    {
                        AssetId = x.DynEntityId,
                        AssetTypeId = x.DynEntityConfigId,
                    });
                return model;
            });
        }

        public ReservationAvailabilityModel GetAssetAvailabilityInDateRange(long assetTypeId, long assetId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException("To be implemented");
        }

        private string GetAssetName(long dynEntityId, long dynEntityConfigId)
        {
            var key = Tuple.Create(dynEntityId, dynEntityConfigId);
            if (!_assetNames.ContainsKey(key))
            {
                var assetType = GetAssetType(dynEntityConfigId);
                var asset = _assetsService.GetAssetById(dynEntityId, assetType);
                _assetNames.Add(key, asset.Name);
            }
            return _assetNames[key];
        }

        private AssetType GetAssetType(long dynEntityConfigId)
        {
            if (!_assetTypes.ContainsKey(dynEntityConfigId))
            {
                var assetType = _assetTypeRepository.GetById(dynEntityConfigId);
                _assetTypes.Add(dynEntityConfigId, assetType);
            }
            return _assetTypes[dynEntityConfigId];
        }

        private Asset GetBorrower(long borrowerId)
        {
            if (!_borrowers.ContainsKey(borrowerId))
            {
                var borrower = _assetsService.GetPredefinedAssetById(borrowerId, PredefinedEntity.User);
                _borrowers.Add(borrowerId, borrower);
            }
            return _borrowers[borrowerId];
        }

        private bool IsAssetReservedForDates(long assetId, long assetTypeId, DateTime startDate, DateTime endDate)
        {
            var reservations = _unitOfWork.ReservationRepository
                .Where(
                    x => ((EntityFunctions.TruncateTime(x.StartDate) <= startDate.Date &&
                          EntityFunctions.TruncateTime(x.EndDate) > startDate.Date)

                      || (EntityFunctions.TruncateTime(x.StartDate) >= startDate.Date &&
                          EntityFunctions.TruncateTime(x.StartDate) < endDate.Date))
                        && x.State == (short)ReservationState.Open
                        && x.ReservedAssets.Any(
                            a => a.DynEntityId == assetId && 
                            a.DynEntityConfigId == assetTypeId));

            return reservations.Any();
        }
    }
}
