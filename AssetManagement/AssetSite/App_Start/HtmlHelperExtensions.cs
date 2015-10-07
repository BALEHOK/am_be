using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace AssetSite
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Gets the application build version.
        /// </summary>
        public static string GetAppBuildVersion(this HtmlHelper helper)
        {
            return GetAppBuildVersion();
        }

        public static string GetAppBuildVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version.ToString();
        }
    }
}