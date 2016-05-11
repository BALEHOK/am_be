using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using AppFramework.Core.Exceptions;

namespace AssetSite.Filters
{
    public class NoPermissionsFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is InsufficientPermissionsException)
            {
                context.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    ReasonPhrase = context.Exception.Message
                };
            }
        }
    }
}