using System.Globalization;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi
{
    public class OutputCacheKeyGenerator : DefaultCacheKeyGenerator
    {
        public override string MakeCacheKey(HttpActionContext context, MediaTypeHeaderValue mediaType,
            bool excludeQueryString = false)
        {
            return base.MakeCacheKey(context, mediaType, excludeQueryString)
                   + ":"
                   + CultureInfo.CurrentUICulture.Name;
        }
    }
}