using System;
using System.Collections.Generic;
using System.Web.Http;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManager.Infrastructure.Services;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    /// <summary>
    /// Returns AssetType-related information
    /// by AssetType Id
    /// </summary>
    [RoutePrefix("api/assettype")]
    public class AssetTypeController : ApiController
    {
        private readonly IAssetService _assetService;
        private readonly ITaxonomyService _taxonomyService;
        private readonly IAssetTypeService _assetTypeService;

        public AssetTypeController(
            IAssetService assetService,
            IAssetTypeService assetTypeService,
            ITaxonomyService taxonomyService)
        {
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;
            if (assetTypeService == null)
                throw new ArgumentNullException("assetTypeService");
            _assetTypeService = assetTypeService;
            if (taxonomyService == null)
                throw new ArgumentNullException("taxonomyService");
            _taxonomyService = taxonomyService;
        }

        [Route(""), HttpGet]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<AssetTypeModel> GetAssetTypes()
        {
            return _assetTypeService.GetAssetTypes(User.GetId());
        }

        [HttpGet]
        [Route("writable")]
        [Route("~/api/writableAssettype")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<AssetTypeModel> GetWritableAssetTypes()
        {
            return _assetTypeService.GetWritableAssetTypes(User.GetId());
        }

        [HttpGet]
        [Route("reservable")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<AssetTypeModel> GetReservableAssetTypes()
        {
            return _assetTypeService.GetReservableAssetTypes(User.GetId());
        }

        [Route("{assetTypeId}"), HttpGet]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public AssetTypeModel GetAssetType(long assetTypeId)
        {
            return _assetTypeService.GetAssetType(User.GetId(), assetTypeId, true);
        }

        /// <summary>
        /// Returns taxonomy path
        /// </summary>
        /// <param name="assetTypeId"></param>
        /// <returns></returns>
        [Route("{assetTypeId}/taxonomy")]
        public TaxonomyModel GetTaxonomy(long assetTypeId)
        {
            return _taxonomyService.GetTaxonomyByAssetTypeId(assetTypeId);
        }

        [Route("{assetTypeId}/child")]
        public IEnumerable<ChildAssetType> GetChildAssetTypes(long assetTypeId)
        {
            return _assetService.GetRelatedAssetTypes(User.GetId(), assetTypeId);
        }
    }
}