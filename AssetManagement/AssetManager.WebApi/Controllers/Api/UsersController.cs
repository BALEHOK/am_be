using System;
using System.Collections.Generic;
using System.Web.Http;
using AppFramework.Core.Exceptions;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using Common.Logging;
using WebApi.OutputCache.V2;
using AppFramework.DataProxy;
using AppFramework.ConstantsEnumerators;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IAssetService _assetService;
        private readonly ILog _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(
            IUnitOfWork unitOfWork,
            IAssetService assetService,
            ILog logger)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        [Route(""), HttpGet]
        [CacheOutput(ServerTimeSpan = 1000, ClientTimeSpan = 1000)]
        public IEnumerable<AssetModel> GetAll(int? rowStart = 0, int? rowsNumber = 20)
        {
            var userId = User.GetId();
            var name = PredefinedEntity.User.ToString();
            var userConfig = _unitOfWork.PredefinedAttributesRepository
                .SingleOrDefault(pa => pa.Name == name);
            if (userConfig == null)
                throw new EntityNotFoundException(string.Format("Cannot find PredefinedEntity {0}", name));
            return _assetService.GetAssets(userConfig.DynEntityConfigID, userId, null, rowStart, rowsNumber);
        }
    }
}
