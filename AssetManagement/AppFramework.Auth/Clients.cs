using System.Collections.Generic;
using System.Configuration;
using AppFramework.Auth.Config;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace AppFramework.Auth
{
    public static class Clients
    {
        private static readonly List<Client> clients; 

        static Clients()
        {
            var AMSPAconfig = AuthConfiguration.Instance.Clients["AMSPA"];
            var AMATconfig = AuthConfiguration.Instance.Clients["AMAT"];
            var AMMconfig = AuthConfiguration.Instance.Clients["AMM"];

            clients = new List<Client>
            {
                // Asset manager frontend
                new Client
                {
                    ClientName = "Asset management SPA",
                    ClientId = "AMSPA",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.Implicit,
                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        AuthConstants.Scopes.WebApi
                    },

                    RedirectUris = AMSPAconfig.RedirectUris,
                    PostLogoutRedirectUris = AMSPAconfig.PostLogoutRedirectUris,

                    ClientSecrets = new List<Secret>(1){ new Secret(ConfigurationManager.AppSettings["AMSPA_Secret"])}
                },

                // Asset manager admin tool
                new Client
                {
                    ClientName = "Asset management admin tool",
                    ClientId = "AMAT",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.Implicit,
                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        AuthConstants.Scopes.WebApi
                    },

                    RedirectUris = AMATconfig.RedirectUris,
                    PostLogoutRedirectUris = AMATconfig.PostLogoutRedirectUris,

                    IdentityTokenLifetime = 86400,
                    AccessTokenLifetime = 86400,

                    ClientSecrets = new List<Secret>(1){ new Secret(ConfigurationManager.AppSettings["AMAT_Secret"])}
                },

                // Asset manager mobile app
                new Client
                {
                    ClientName = "Asset management mobile app",
                    ClientId = "AMM",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,
                    Flow = Flows.Implicit,
                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        AuthConstants.Scopes.WebApi
                    },

                    RedirectUris = AMMconfig.RedirectUris,
                    PostLogoutRedirectUris = AMMconfig.PostLogoutRedirectUris,

                    ClientSecrets = new List<Secret>(1){ new Secret(ConfigurationManager.AppSettings["AMM_Secret"])}
                }
            };
        }

        public static List<Client> Get()
        {
            return clients;
        }
    }
}