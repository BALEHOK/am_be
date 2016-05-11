using AppFramework.Reservations.Models;
using AppFramework.Reservations.Services;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/reservations/{reservationId}/assets")]
    public class ReservationAssetsController : ApiController
    {
        private readonly ILog _logger;
        private readonly ReservationsService _reservationsService;

        public ReservationAssetsController(
            ReservationsService reservationsService,
            ILog logger)
        {
            if (reservationsService == null)
                throw new ArgumentNullException("reservationsService");
            _reservationsService = reservationsService;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        /// <summary>
        /// Returns list of reservations
        /// </summary>
        /// <returns></returns>
        [Route(""), HttpGet]
        [CacheOutput(ServerTimeSpan = 5, ClientTimeSpan = 5)]
        public IEnumerable<ReservedAssetModel> GetReservationAssets(long reservationId)
        {
            return _reservationsService.GetReservationAssets(reservationId);
        }
    }
}
