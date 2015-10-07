using System.Linq;

namespace AssetSite
{
    public class EFCachingConfig
    {
        public static void SetCaching()
        {
            EFCachingProvider.EFCachingProviderConfiguration.DefaultCache = new EFCachingProvider.Web.AspNetCache();
            var cachingPolicy = new EFCachingProvider.Caching.CustomCachingPolicy();
            string[] cachingTables =
            {
                "Context", 
                "DataType", 
                "Languages", 
                "Role", 
                "ScreenLayout", 
                "StringResources", 
                "PredefinedAsset", 
                "PredefinedAttributes"
            };
            cachingTables.ToList().ForEach(ct => cachingPolicy.CacheableTables.Add(ct));
            EFCachingProvider.EFCachingProviderConfiguration.DefaultCachingPolicy = cachingPolicy;
        }
    }
}