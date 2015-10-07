using AppFramework.DataProxy;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public RolesController(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        [Route("")]
        [CacheOutput(ServerTimeSpan = 60 * 60, ClientTimeSpan = 60 * 60)]
        public IEnumerable<RoleModel> Get()
        {
            return from role in _unitOfWork.RoleRepository.Get()
                   select new RoleModel
                   {
                       Id = role.RoleId,
                       Name = role.RoleName
                   };
        }
    }
}