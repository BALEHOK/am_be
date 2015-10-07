using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using AppFramework.Core.Interfaces;

namespace AssetSite
{
    public class UnityHttpHandlerFactory : IHttpHandlerFactory
    {
        public UnityHttpHandlerFactory()
        {
        }

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            var container = ((IHttpUnityApplication)context.ApplicationInstance).UnityContainer;
            return container.Resolve<IHttpHandler>(url);
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}