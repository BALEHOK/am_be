using AppFramework.DataProxy;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/zipcodes")]
    public class ZipCodesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ZipCodesController(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        [CacheOutput(ServerTimeSpan = 60 * 60, ClientTimeSpan = 60 * 60)]
        public IEnumerable<ZipCodeModel> Get(string query = null, int? rowStart = 1, int? rowsNumber = 20)
        {
            var querySet = _unitOfWork.ZipCodeRepository
                .AsQueryable()
                .OrderBy(c => c.Code)
                .AsQueryable();

            if (query != null)
                querySet = querySet.Where(p => p.Code.ToLower().StartsWith(query.ToLower()));
            if (rowStart.HasValue)
                querySet = querySet.Skip(rowStart.Value);
            if (rowsNumber.HasValue)
                querySet = querySet.Take(rowsNumber.Value);

            return from code in querySet
                   select new ZipCodeModel
                   {
                       Id = code.ZipId,
                       Code = code.Code
                   };
        }
    }
}