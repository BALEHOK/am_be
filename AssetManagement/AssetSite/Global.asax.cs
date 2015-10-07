using System.Web.Mvc;
using System.Web.Routing;
using AppFramework.Core.Interfaces;
using Common.Logging;
using System;
using System.Web;
using Microsoft.Practices.Unity;

namespace AssetSite
{
    public class Global : HttpApplication, IHttpUnityApplication
    {
        // This implements the IHttpUnityApplication interface
        public IUnityContainer UnityContainer
        {
            get { return _unityContainer ?? (_unityContainer = new UnityContainer()); }
        }

        // This is static because it is normal for ASP.NET to create
        // several HttpApplication objects (pooling) but only the first
        // will run Application_Start(), which is where this is set.
        private static IUnityContainer _unityContainer;

        void Application_Start(object sender, EventArgs e)
        {
            UnityConfig.RegisterComponents(UnityContainer);            
            EFCachingConfig.SetCaching();
            AutoMapperConfig.RegisterDataMappings();
            LoggerConfig.Configure();
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!AssetSite.Helpers.CookieWrapper.IsSet)
            {
                AssetSite.Helpers.CookieWrapper.SetCookie();
            }
            if (AssetSite.Helpers.CookieWrapper.Language == string.Empty)
            {
                AssetSite.Helpers.CookieWrapper.Language = "en-US";
            }
        }
      
        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = HttpContext.Current.Server.GetLastError();
            LogManager.GetCurrentClassLogger().Error(ex);
        }
    }
}