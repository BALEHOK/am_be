using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Helpers;
using AssetManager.Infrastructure.Models;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/dynlist")]
    public class DynListController : ApiController
    {
        private readonly IDynamicListsService _dynListService;

        public DynListController(IDynamicListsService dynListService)
        {
            if (dynListService == null)
                throw new ArgumentNullException("dynListService");
            _dynListService = dynListService;
        }

        [Route("")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<DynListModel> GetAll()
        {
            return from list in _dynListService.GetAll()
                select list.ToDto();
        }

        [Route("{dynamicListUid}")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public DynListModel GetByUid(long dynamicListUid)
        {
            var entity = _dynListService.GetByUid(dynamicListUid);
            return entity.ToDto();
        }

        [Route("attribute/{attributeId}")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public DynListModel GetDynListByAttributeId(long attributeId)
        {
            var entity = _dynListService.GetByAttributeId(attributeId);
            return entity.ToDto();
        }
    }
}