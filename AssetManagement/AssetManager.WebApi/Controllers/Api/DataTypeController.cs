using AppFramework.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/datatype")]
    public class DataTypeController : ApiController
    {
        [Route("")]
        public IEnumerable<string> Get()
        {
            return Enum.GetNames(typeof(Enumerators.DataType));
        }
    }
}
