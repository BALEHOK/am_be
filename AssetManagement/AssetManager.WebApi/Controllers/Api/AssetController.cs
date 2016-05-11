using System;
using System.Collections.Generic;
using System.Web.Http;
using AppFramework.Core.Exceptions;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Helpers;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using AssetManager.WebApi.Validators;
using Common.Logging;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/assettype/{assetTypeId}/asset")]
    public class AssetController : ApiController
    {
        private readonly IAssetService _assetService;
        private readonly ILog _logger;

        public AssetController(
            IAssetService assetService,
            ILog logger)
        {
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        [Route(""), HttpGet]
        public AssetModel GetEmptyModel(long assetTypeId)
        {
            var userId = User.GetId();
            return _assetService.CreateAsset(assetTypeId, userId);
        }

        [Route("{assetId}")]
        [Route("{assetId}/revisions/{revision}")]
        public AssetModel GetById(long assetTypeId, long assetId, int? revision = null, long? uid = null)
        {
            return _assetService.GetAsset(assetTypeId, assetId, User.GetId(), revision, uid, true);
        }

        [Route(""), HttpPut] // TODO : this should be POST
        [Route("{assetId}"), HttpPost] // and this should be PUT
        [ValidateModelState, CheckModelForNull]
        public IHttpActionResult Save(AssetModel model, long screenId)
        {
            var userId = User.GetId();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _assetService.SaveAsset(model, userId, screenId);
                    return Ok(new {Id = result.Item1, Name = result.Item2});
                }
                catch (AssetValidationException ex)
                {
                    ModelState.AddModelErrors(ex);
                }
                catch (InvalidFormulaException ex)
                {
                    ModelState.AddModelErrors(ex);
                }
                catch (InsufficientPermissionsException ex)
                {
                    _logger.Debug(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return InternalServerError();
                }
            }
            return BadRequest(ModelState);
        }

        [Route("{assetId}/related")]
        [Route("related")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<AssetAttributeRelatedEntitiesModel> GetRelatedEntities(
            long assetTypeId, long? assetId = null, int? revision = null, long? uid = null)
        {
            return _assetService.GetAssetRelatedEntities(assetTypeId, assetId, revision, uid);
        }

        [Route("{assetId}/history")]
        [Route("{assetId}/revisions")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public AssetHistoryModel GetHistory(long assetTypeId, long assetId)
        {
            return _assetService.GetAssetHistory(assetTypeId, assetId);
        }

        [Route("{assetId}"), HttpDelete]
        public void Delete(long assetTypeId, long assetId)
        {
            _assetService.DeleteAsset(assetTypeId, assetId, User.GetId());
        }

        [Route("{assetId}/restore"), HttpPost]
        public void Restore(long assetTypeId, long assetId)
        {
            _assetService.RestoreAsset(assetTypeId, assetId);
        }

        [HttpPost]
        [Route("{assetId}/calculate")]
        [Route("calculate")]
        [ValidateModelState, CheckModelForNull]
        public IHttpActionResult Calculate(AssetModel model, long screenId)
        {
            var userId = User.GetId();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _assetService.CalculateAsset(model, userId, screenId);
                    return Ok(result);
                }
                catch (AssetValidationException ex)
                {
                    ModelState.AddModelErrors(ex);
                }
                catch (InvalidFormulaException ex)
                {
                    ModelState.AddModelErrors(ex);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return InternalServerError();
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Returns list of assets for given type
        /// </summary>
        /// <param name="assetTypeId">AssetType Id</param>
        /// <param name="query">Filter by asset name</param>
        /// <param name="rowStart">Offset</param>
        /// <param name="rowsNumber">Items to return</param>
        /// <returns></returns>
        [Route("~/api/assettype/{assetTypeId}/assets")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<AssetModel> GetAllAssets(
            long assetTypeId, string query = null, int? rowStart = 0, int? rowsNumber = 20)
        {
            // TODO: impove this API by adding strong-typed property
            var userId = User.GetId();
            return _assetService.GetAssetsByName(assetTypeId, userId, query, rowStart, rowsNumber);
        }
    }
}