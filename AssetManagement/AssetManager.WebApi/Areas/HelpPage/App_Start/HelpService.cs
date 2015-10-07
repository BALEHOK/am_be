using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace AssetManager.WebApi.Areas.HelpPage
{
    public class HelpService
    {
        public static string GetHttpMethod(ApiDescription api)
        {
            return api.HttpMethod.Method;
        }
    }
}