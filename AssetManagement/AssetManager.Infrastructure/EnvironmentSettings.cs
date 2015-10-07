
using System.Configuration;
using System.Web;

namespace AssetManager.Infrastructure
{
    public class EnvironmentSettings : IEnvironmentSettings
    {
        public string PageNumber { get; set; }
        public string PageSize { get; set; }

        private const string QUOTE = "\"";
        private const string ESCAPED_QUOTE = "\"\"";
        private static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };

        public EnvironmentSettings()
        {
            PageNumber = "PageNumber";
            PageSize = "PageSize";
        }

        public string GetSiteRoot()
        {
            //Formatting the fully qualified website url/name 
            return string.Format("{0}://{1}{2}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Host,
                HttpContext.Current.Request.Url.Port == 80
                    ? string.Empty
                    : ":" + HttpContext.Current.Request.Url.Port);
        }

        public string GetPathToAssetPage(long assetTypeId, long assetId)
        {
            return string.Format("{0}/#/assettype/{1}/asset/{2}",
                GetSiteRoot(), assetTypeId, assetId);
        }

        public string Escape(string s)
        {
            if (s.Contains(QUOTE))
                s = s.Replace(QUOTE, ESCAPED_QUOTE);

            if (s.IndexOfAny(CHARACTERS_THAT_MUST_BE_QUOTED) > -1)
                s = QUOTE + s + QUOTE;

            return s;
        }

        private static string _unescape(string s)
        {
            if (s.StartsWith(QUOTE) && s.EndsWith(QUOTE))
            {
                s = s.Substring(1, s.Length - 2);

                if (s.Contains(ESCAPED_QUOTE))
                    s = s.Replace(ESCAPED_QUOTE, QUOTE);
            }

            return s;
        }

        public string GetDocsUploadDirectory(long assetTypeId, long attributeId)
        {
            var baseDir = ConfigurationManager.AppSettings["DocsUploadDir"] ?? "~/App_Data/uploads";
            return string.Format(@"{0}/assets/{1}/{2}", 
                baseDir, assetTypeId, attributeId);
        }

        public string GetCacheDirectory()
        {
            return ConfigurationManager.AppSettings["CacheDir"] ?? "~/App_Data/cache";
        }

        public string GetImagesUploadDirectory(long assetTypeId, long attributeId)
        {
            var baseDir = ConfigurationManager.AppSettings["ImagesUploadDir"] ?? "~/uploads";
            return string.Format(@"{0}/assets/{1}/{2}", 
                baseDir, assetTypeId, attributeId);
        }
    }
}