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
        public TypesInfoModel GetAssetTypes()
        {
            return _assetTypeService.GetAssetTypes();
        }

        [Route("{assetTypeId}"), HttpGet]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public AssetTypeModel GetAssetType(long assetTypeId)
        {
            return _assetTypeService.GetAssetType(assetTypeId, true);
        }

        /// <summary>
        /// Returns list of assets for given type
        /// </summary>
        /// <param name="assetTypeId">AssetType Id</param>
        /// <param name="query">Filter by asset name</param>
        /// <param name="rowStart">Offset</param>
        /// <param name="rowsNumber">Items to return</param>
        /// <returns></returns>
        [Route("{assetTypeId}/assets")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<AssetModel> GetAllAssets(
            long assetTypeId, string query = null, int? rowStart = 1, int? rowsNumber = 20)
        {
            // TODO: impove this API by adding strong-typed property
            var userId = User.GetId();
            return _assetService.GetAssets(assetTypeId, userId, query, rowStart, rowsNumber);
        }

        /// <summary>
        /// Returns taxonomy path
        /// </summary>
        /// <param name="assetTypeId"></param>
        /// <returns></returns>
        [Route("{assetTypeId}/taxonomy")]
        public IEnumerable<TaxonomyModel> GetTaxonomy(long assetTypeId)
        {
            return _taxonomyService.GetTaxonomyByAssetTypeId(assetTypeId);
        }

        [Route("{assetTypeId}/child")]
        public IEnumerable<ChildAssetType> GetChildAssetTypes(long assetTypeId)
        {
            return _assetService.GetChildAssetTypes(assetTypeId);
        }
    }
}