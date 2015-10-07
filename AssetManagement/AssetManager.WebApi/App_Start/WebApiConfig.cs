using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using AssetSite.Filters;

namespace AssetManager.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // routing with class and method attributes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver
                = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            jsonFormatter.SerializerSettings.PreserveReferencesHandling =
                Newtonsoft.Json.PreserveReferencesHandling.Objects;

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // configure filters
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new AssetNotFoundFilter());
            config.Filters.Add(new AssetTypeNotFoundFilter());
            config.Filters.Add(new DynListNotFoundFilter());
            config.Filters.Add(new NoPermissionsFilter());
            config.Filters.Add(new ArgumentExceptionFilter());
            config.Filters.Add(new LogExceptionFilter());
        }
    }
}
