using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using IActionFilter = System.Web.Http.Filters.IActionFilter;

namespace AssetSite.Filters
{
    public class UiCultureFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public bool AllowMultiple
        {
            get { return false; }
        }

        public Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var acceptedLangs = actionContext.Request.Headers.AcceptLanguage;
            if (acceptedLangs.Count == 0)
            {
                return continuation();
            }

            // try to find lang with Quality==null first,
            // if no such lang found, take one with the highest Quality, but greater than 0
            // https://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.9
            var header = acceptedLangs.FirstOrDefault(l => !l.Quality.HasValue && !string.IsNullOrEmpty(l.Value))
                         ?? acceptedLangs.OrderByDescending(l => l.Quality)
                             .FirstOrDefault(l => l.Quality > 0 && !string.IsNullOrEmpty(l.Value));
            
            if (header != null)
            {
                CultureInfo uiCulture;
                try
                {
                    uiCulture = CultureInfo.CreateSpecificCulture(header.Value);
                }
                catch (ArgumentException)
                {
                    return continuation();
                }

                Thread.CurrentThread.CurrentUICulture = uiCulture;
            }

            return continuation();
        }
    }
}