using AppFramework.DataProxy;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/places")]
    public class PlacesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlacesController(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        [CacheOutput(ServerTimeSpan = 60 * 60, ClientTimeSpan = 60 * 60)]
        public IEnumerable<PlaceModel> Get(string filter = null, int? rowStart = 1, int? rowsNumber = 20)
        {
            var querySet = _unitOfWork.PlaceRepository
                .AsQueryable()
                .OrderBy(p => p.PlaceName)
                .AsQueryable();

            if (filter != null)
                querySet = querySet.Where(p => p.PlaceName.ToLower().StartsWith(filter.ToLower()));
            if (rowStart.HasValue)
                querySet = querySet.Skip(rowStart.Value);
            if (rowsNumber.HasValue)
                querySet = querySet.Take(rowsNumber.Value);

            return from place in querySet
                   select new PlaceModel
                   {
                       Id = place.PlaceId,
                       Name = place.PlaceName
                   };
        }
    }
}