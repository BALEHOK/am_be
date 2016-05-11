using AppFramework.Reservations.Models;
using AppFramework.Reservations.Services;
using AssetManager.Infrastructure.Extensions;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/reservations")]
    public class ReservationsController : ApiController
    {
        private readonly ILog _logger;
        private readonly ReservationsService _reservationsService;

        public ReservationsController(
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
        [CacheOutput(ServerTimeSpan = 5, ClientTimeSpan = 100)]
        public IEnumerable<ReservationModel> GetAll()
        {
            return _reservationsService.GetAllReservations();
        }

        /// <summary>
        /// Returns reservation by its id
        /// </summary>
        /// <returns></returns>
        [Route("{reservationId}"), HttpGet]
        [CacheOutput(ServerTimeSpan = 5, ClientTimeSpan = 5)]
        public ReservationModel GetById(long reservationId)
        {
            return _reservationsService.GetReservationById(reservationId);
        }

        /// <summary>
        /// Creates a new reservation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(""), HttpPost]
        public HttpResponseMessage Create(ReservationModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _reservationsService.CreateReservation(model, User.GetId());
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        /// <summary>
        /// Updates a reservation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{reservationId}"), HttpPut]
        public ReservationModel Update(long reservationId, ReservationModel model)
        {
            return _reservationsService.UpdateReservation(reservationId, model, User.GetId());
        }

        /// <summary>
        /// Changes reservation status
        /// </summary>
        /// <returns></returns>
        [Route("{reservationId}/status/{state}"), HttpPut]
        public IHttpActionResult ChangeStatus(long reservationId, short state, [FromBody]string comment)
        {
            _reservationsService.ChangeStatus(reservationId, state, comment, User.GetId());
            return Ok(new { state = state, id = reservationId });
        }

        /// <summary>
        /// Deletes a reservation
        /// </summary>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        [Route("{reservationId}"), HttpDelete]
        public ReservationModel Delete(long reservationId)
        {
            throw new NotImplementedException();
        }
    }
}
