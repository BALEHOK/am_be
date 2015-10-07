using Microsoft.Practices.Unity.WebApi;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AssetManager.WebApi.UnityActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(AssetManager.WebApi.UnityActivator), "Shutdown")]

namespace AssetManager.WebApi
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class UnityActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() 
        {
            var container = UnityConfig.GetConfiguredContainer();
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver 
                = new CustomUnityResolver(container);
            System.Web.Mvc.DependencyResolver.SetResolver(
                new Unity.Mvc5.UnityDependencyResolver(container));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}
