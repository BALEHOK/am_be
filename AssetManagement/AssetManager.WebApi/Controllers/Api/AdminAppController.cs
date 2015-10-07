using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManager.Infrastructure.Services;

namespace AssetManager.WebApi.Controllers.Api
{
    /// <summary>
    /// Administration app API
    /// </summary>
    [RoutePrefix("api/typesinfo")]   
    public class AdminAppController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeService _assetTypeService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="screensService"></param>
        /// <param name="dataFactory"></param>
        public AdminAppController(IUnitOfWork unitOfWork, IAssetTypeService assetTypeService)
        {
            if (unitOfWork == null)
                throw new System.ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeService == null)
                throw new System.ArgumentNullException("assetTypeService");
            _assetTypeService = assetTypeService;
        }

        /// <summary>
        /// Gets all active asset types including screens info
        /// </summary>
        /// <returns></returns>
        [Route(""), HttpGet]
        public TypesInfoModel GetTypesInfo()
        {
            return _assetTypeService.GetAssetTypes(true, true);
        }

        private string UpdateAttributeParameter(string typeId, string attributeName, string parameterName, object value)
        {
            var id = long.Parse(typeId);
            var config =
                _unitOfWork.DynEntityConfigRepository.Single(
                    c => c.ActiveVersion && c.DynEntityConfigId == id, c => c.DynEntityAttribConfigs);

            var attribute = config.DynEntityAttribConfigs.Single(a => a.DBTableFieldname == attributeName);

            var paremeters = new IDataParameter[]
            {
                new SqlParameter("@value", value),
                new SqlParameter("@uid", attribute.DynEntityAttribConfigUid)
            };

            var query =
                string.Format(
                    "UPDATE [DynEntityAttribConfig] SET [{0}] = @value WHERE [DynEntityAttribConfigUid] = @uid", parameterName);
            _unitOfWork.SqlProvider.ExecuteNonQuery(query, paremeters);

            return string.Format("[{0}].[{1}]", config.Name, attribute.Name);
        }

        /// <summary>
        /// Removes attribute formula
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        [Route("formula/clear"), HttpGet]
        public string ClearFormula(string typeId, string attributeName)
        {
            string response;
            try
            {
                var updatedName = UpdateAttributeParameter(typeId, attributeName, "CalculationFormula", string.Empty);
                response = string.Format("Removed formula for {0}", updatedName);
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }

            return response;
        }

        /// <summary>
        /// Upadtes attribute formula
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="attributeName"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        [Route("formula"), HttpGet]
        public string SaveFormula(string typeId, string attributeName, string formula)
        {
            string response;
            try
            {
                var updatedName = UpdateAttributeParameter(typeId, attributeName, "CalculationFormula", formula);
                response = string.Format("Saved formula for {0}", updatedName);
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }

            return response;
        }

        private AttributePanelAttribute UpdateScreenFormula(long panelAttributeId, string formula)
        {
            var panelAttribute =
                _unitOfWork.AttributePanelAttributeRepository.SingleOrDefault(
                    apa => apa.AttributePanelAttributeId == panelAttributeId, apa => apa.DynEntityAttribConfig);

            panelAttribute.ScreenFormula = formula;
            _unitOfWork.Commit();

            return panelAttribute;
        }

        /// <summary>
        /// Updates attribute screen formula
        /// </summary>
        /// <param name="panelAttributeId"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        [Route("screens/formula"), HttpGet]
        public string SaveScreenFormula(long panelAttributeId, string formula)
        {
            var panelAttribute = UpdateScreenFormula(panelAttributeId, formula);
            return string.Format("Saved screen formula for attribute '{0}'", panelAttribute.DynEntityAttribConfig.Name);
        }

        /// <summary>
        /// Removes attribute screen formula
        /// </summary>
        /// <param name="panelAttributeId"></param>
        /// <returns></returns>
        [Route("screens/formula/clear"), HttpGet]
        public string ClearScreenFormula(long panelAttributeId)
        {
            var panelAttribute = UpdateScreenFormula(panelAttributeId, null);
            return string.Format("Removed screen formula for attribute '{0}'", panelAttribute.DynEntityAttribConfig.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        [Route("validation/clear"), HttpGet]
        public string ClearValidationExpression(string typeId, string attributeName)
        {
            string response;
            try
            {
                var updatedName = UpdateAttributeParameter(typeId, attributeName, "ValidationExpr", string.Empty);
                response = string.Format("Removed validator for {0}", updatedName);
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="attributeName"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Route("validation"), HttpGet]
        public string SaveValidationExpression(string typeId, string attributeName, string expression)
        {
            string response;

            try
            {
                var updatedName = UpdateAttributeParameter(typeId, attributeName, "ValidationExpr", expression);
                response = string.Format("Saved validator for {0}", updatedName);
            }
            catch (Exception exception)
            {
                response = exception.ToString();
            }

            return response;
        }
    }
}