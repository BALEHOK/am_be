using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.ConstantsEnumerators;
using AssetManager.Infrastructure.Models.Search;
using AssetManager.Infrastructure.Services;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/datatype")]
    public class DataTypeController : ApiController
    {
        private readonly IDataTypeService _dataTypeService;

        public DataTypeController(IDataTypeService dataTypeService)
        {
            if (dataTypeService == null)
                throw new ArgumentNullException("dataTypeService");
            _dataTypeService = dataTypeService;
        }

        [Route("")]
        public IEnumerable<string> Get()
        {
            return Enum.GetNames(typeof (Enumerators.DataType));
        }

        [Route("{typeName}/operators"), HttpGet]
        public IEnumerable<object> GetAttributeTypeOperators(string typeName)
        {
            return _dataTypeService.GetDataTypeOperators(typeName)
                .Select(o => new IdNamePair<long, string>(o.SearchOperatorUid, o.Operator));
        }
    }
}