using AppFramework.Core.Services;
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
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            if (roleService == null)
                throw new ArgumentNullException("roleService");
            _roleService = roleService;
        }

        [Route("")]
        [CacheOutput(ServerTimeSpan = 60 * 60, ClientTimeSpan = 60 * 60)]
        public IEnumerable<RoleModel> Get()
        {
            return from role in _roleService.GetAllRoles()
                   select new RoleModel
                   {
                       Id = (int)role,
                       Name = role.ToString()
                   };
        }
    }
}