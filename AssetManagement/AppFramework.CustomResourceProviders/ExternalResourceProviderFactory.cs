using System;
using System.Web.Compilation;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

namespace AppFramework.CustomResourceProviders
{

    /// <summary>
    /// Provider factory implementation for external resources. Only supports
    /// global resources. 
    /// </summary>
    public class ExternalResourceProviderFactory : ResourceProviderFactory
    {

        public override IResourceProvider CreateGlobalResourceProvider(string classKey)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "ExternalResourceProviderFactory.CreateGlobalResourceProvider({0})", classKey));

            return new GlobalExternalResourceProvider(classKey);
        }

        public override IResourceProvider CreateLocalResourceProvider(string virtualPath)
        {
            throw new NotSupportedException(String.Format(Thread.CurrentThread.CurrentUICulture, Properties.Resources.Provider_LocalResourcesNotSupported, "ExternalResourceProviderFactory"));
        }
    }
}