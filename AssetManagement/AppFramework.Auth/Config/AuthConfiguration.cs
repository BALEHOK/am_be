using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace AppFramework.Auth.Config
{
    public class AuthConfiguration
    {
        private static readonly Lazy<AuthConfiguration> _instance = new Lazy<AuthConfiguration>(LoadConfig);

        public static AuthConfiguration Instance
        {
            get { return _instance.Value; }
        }

        private AuthConfiguration()
        {
        }

        private static AuthConfiguration LoadConfig()
        {
            string jsonString;
            using (var r = new StreamReader(HttpRuntime.BinDirectory + "auth.config.json"))
            {
                jsonString = r.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<AuthConfiguration>(jsonString);
        }

        public string AuthServerUrl { get; set; }
        public AuthCertificate Certificate { get; set; }
        public string[] CorsAllowedOrigins { get; set; }
        public Dictionary<string, AuthClient> Clients { get; set; }

        #region Helper classes

        public class AuthCertificate
        {
            public string File { get; set; }
            public string Password { get; set; }
        }

        public class AuthClient
        {
            public List<string> RedirectUris { get; set; }
            public List<string> PostLogoutRedirectUris { get; set; }
        }

        #endregion Helper classes
    }
}