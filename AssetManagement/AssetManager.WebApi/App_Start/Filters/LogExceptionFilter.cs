using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace AssetSite.Filters
{
    public class LogExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException)
            {
                var httpException = context.Exception as HttpResponseException;
                var message = string.Format("{0}\n{1}",
                    httpException.Response.ReasonPhrase,
                    httpException.Response.Content.ReadAsStringAsync().Result);
                _logger.Error(
                    message, 
                    httpException);
            }
            else
            {
                string message = string.Format(@"
                    Request Uri: {0}
                    Error details: {1}",
                        context.Request.RequestUri,
                        context.Exception.Message);

                _logger.Error(message, context.Exception);

                context.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = "An error occurred, please try again or contact the administrator."
                };
            }
        }
    }
}