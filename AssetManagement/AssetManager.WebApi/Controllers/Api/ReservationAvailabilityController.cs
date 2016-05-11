using AppFramework.Core.Classes;
using AppFramework.Reservations.Models;
using AppFramework.Reservations.Services;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    /// <summary>
    /// Reservations and availability
    /// </summary>
    [RoutePrefix("api/availability")]
    public class ReservationAvailabilityController : ApiController
    {
        private readonly ReservationsService _reservationsService;
        private readonly IAssetService _assetService;

        public ReservationAvailabilityController(
            IAssetService assetService,
            ReservationsService reservationsService)
        {
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;

            if (reservationsService == null)
                throw new ArgumentNullException("reservationsService");
            _reservationsService = reservationsService;
        }

        [Route("assettypes/{assetTypeId}/assets")]
        [CacheOutput(ServerTimeSpan = 5, ClientTimeSpan = 5)]
        public IEnumerable<AssetModel> GetAvailableAssetsInDateRange(
            long assetTypeId, DateTime startDate, DateTime endDate, int? rowStart = 0, int? rowsNumber = 20)
        {
            var userId = User.GetId();

            var reservations = _reservationsService.GetReservationsByAssetTypeIdInDateRange(
                assetTypeId, startDate, endDate);

            var allReservedAssetIds = reservations
                .SelectMany(x => x.ReservedAssets)
                .Select(x => new {AssetId = x.AssetId, ConfigId = x.AssetTypeId})
                .ToList();

            Func<Asset, bool> filterPredicate = (asset) =>
            {
                return allReservedAssetIds.TrueForAll(x => !(x.AssetId == asset.ID && x.ConfigId == asset.Configuration.ID));
            };
            return _assetService.GetAssets(assetTypeId, userId, filterPredicate, rowStart, rowsNumber);
        }

        [Route("assettypes/{assetTypeId}/assets/{assetId}")]
        public ReservationAvailabilityModel GetAssetAvailabilityInDateRange(long assetTypeId, long assetId, DateTime startDate, DateTime endDate)
        {
            return _reservationsService.GetAssetAvailabilityInDateRange(assetTypeId, assetId, startDate, endDate);
        }
    }
}
