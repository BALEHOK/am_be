using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;
using AppFramework.Core.Exceptions;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Helpers;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using AssetManager.WebApi.Validators;
using WebApi.OutputCache.V2;
using Common.Logging;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/assettype/{assetTypeId}/asset/{assetId}")]
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

        [Route("")]
        [Route("revisions/{revision}")]
        public AssetModel Get(long assetTypeId, long assetId, int? revision = null, long? uid = null)
        {
            return _assetService.GetAsset(assetTypeId, assetId, revision, uid, true);
        }

        [Route("~/api/assettype/{assetTypeId}/asset"), HttpGet]
        public AssetModel Create(long assetTypeId)
        {
            var userId = User.GetId();
            return _assetService.CreateAsset(assetTypeId, userId);
        }
     
        [Route(""), HttpPost]
        [Route("~/api/assettype/{assetTypeId}/asset"), HttpPut]
        [ValidateModelState, CheckModelForNull]
        public IHttpActionResult Save(AssetModel model, long screenId)
        {
            var userId = User.GetId();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _assetService.SaveAsset(model, userId, screenId);
                    return Ok(new { Id = result.Item1, Name = result.Item2 });
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

        [Route("related")]
        [Route("~/api/assettype/{assetTypeId}/asset/related")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public IEnumerable<AssetAttributeRelatedEntitiesModel> GetRelatedEntities(
            long assetTypeId, long? assetId = null, int? revision = null, long? uid = null)
        {
            return _assetService.GetAssetRelatedEntities(assetTypeId, assetId, revision, uid);
        }

        [Route("history"), Route("revisions")]
        [CacheOutput(ServerTimeSpan = 100, ClientTimeSpan = 100)]
        public AssetHistoryModel GetHistory(long assetTypeId, long assetId)
        {
            return _assetService.GetAssetHistory(assetTypeId, assetId);
        }

        [Route(""), HttpDelete]
        public void Delete(long assetTypeId, long assetId)
        {
            _assetService.DeleteAsset(assetTypeId, assetId);
        }

        [Route("restore"), HttpPost]
        public void Restore(long assetTypeId, long assetId)
        {
            _assetService.RestoreAsset(assetTypeId, assetId);
        }

        [HttpPost]
        [Route("calculate")]
        [Route("~/api/assettype/{assetTypeId}/asset/calculate")]
        [ValidateModelState, CheckModelForNull]
        public IHttpActionResult Calculate(AssetModel model, long screenId, bool forceRecalc = false)
        {
            var userId = User.GetId();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _assetService.CalculateAsset(model, userId, screenId, forceRecalc);
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
    }
}