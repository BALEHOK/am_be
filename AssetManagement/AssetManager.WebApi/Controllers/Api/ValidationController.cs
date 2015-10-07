using System.Web.Http;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Validation;
using AppFramework.DataProxy;
using AssetManager.Infrastructure.Models;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using AssetManager.Infrastructure;
using AssetManager.WebApi.Extensions;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/validation")]
    public class ValidationController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationServiceNew _validationService;
        private readonly IDataFactory _dataFactory;
        private readonly IModelFactory _attributeModelFactory;

        public ValidationController(
            IUnitOfWork unitOfWork, 
            IValidationServiceNew validationService,
            IDataFactory dataFactory,
            IModelFactory attributeModelFactory)
        {
            _unitOfWork = unitOfWork;
            _validationService = validationService;
            _dataFactory = dataFactory;
            _attributeModelFactory = attributeModelFactory;
        }

        /// <summary>
        /// Validates attribute. Takes last attribute config version
        /// </summary>
        /// <param name="id">attribute id</param>
        /// <param name="value">attribute value</param>
        /// <param name="expression">if this parameter is empty expression will be taken from attribute's config</param>
        /// <returns></returns>
        [Route("attribute/{id}"), HttpPost]
        public AttributeValidationResultModel ValidateAttribute(
            [FromUri]long id,
            [FromBody]JToken value,
            [FromUri]string expression = null)
        {            
            var assetAttribute = _dataFactory.Get<AssetAttribute>(id);
            _attributeModelFactory.AssignValue(assetAttribute, value);

            if (!string.IsNullOrEmpty(expression))
                assetAttribute.Configuration.ValidationExpr = expression;

            var userId = User.GetId();
            var validationResult = _validationService.ValidateAttribute(assetAttribute, userId);

            var response = new AttributeValidationResultModel
            {
                Id = id,
                IsValid = validationResult.IsValid,
                Message = validationResult.GetErrorMessage("\r\n"),
            };

            return response;
        }
    }
}