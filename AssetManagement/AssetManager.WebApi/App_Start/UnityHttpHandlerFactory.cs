using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using AppFramework.Core.Interfaces;

namespace AssetManager.WebApi
{
    public class UnityHttpHandlerFactory : IHttpHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            var container = UnityConfig.GetConfiguredContainer();
            return container.Resolve<IHttpHandler>(url);
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}