using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetSite.Helpers
{
    /// <summary>
    /// Manages site cookies
    /// </summary>
    public static class CookieWrapper
    {

        #region Properties

        private static string _cookieKey = "amcookie";

        private static HttpCookie _cookie
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[_cookieKey];
                if (cookie == null) cookie = SetCookie();
                return cookie;
            }
        }

        /// <summary>
        /// Gets if cookies were set for this site.
        /// </summary>
        public static bool IsSet
        {
            get
            {
                return _cookie != null;
            }
        }

        /// <summary>
        /// Gets the current language. 
        /// </summary>
        /// <returns></returns>
        public static string Language
        {
            get
            {
                return _cookie["language"] == null ? string.Empty : _cookie["language"].ToString();
            }
            set
            {
                _cookie["language"] = value;
                HttpContext.Current.Response.Cookies.Add(_cookie);
            }
        }
        #endregion

        /// <summary>
        /// Sets the site cookie.
        /// </summary>
        public static HttpCookie SetCookie()
        {
            HttpCookie cookie = new HttpCookie(_cookieKey);
            cookie.Expires = DateTime.Now.AddYears(5);
            HttpContext.Current.Response.Cookies.Add(cookie);
            return cookie;
        }

        /// <summary>
        /// Unsets the site cookie.
        /// </summary>
        public static void UnsetCookie()
        {
            _cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(_cookie);
        }
    }
}
