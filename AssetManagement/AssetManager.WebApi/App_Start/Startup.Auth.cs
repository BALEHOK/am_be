using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using AppFramework.Auth;
using AppFramework.Auth.Config;
using IdentityServer3.AccessTokenValidation;
using log4net;
using Microsoft.Owin.Cors;
using Owin;

namespace AssetManager.WebApi
{
    public partial class Startup
    {
        /// <summary>
        /// Authentication config
        /// </summary>
        public void ConfigureAuth(IAppBuilder app)
        {
            // allow all only for debug and development
            // ToDo [Alexandr Shukletsov] Validate CORS
            app.UseCors(CorsOptions.AllowAll);

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            // accept access tokens from identityserver and require a scope of 'api1'
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = AuthConfiguration.Instance.AuthServerUrl,
                RequiredScopes = new[] {"webApi"},
                NameClaimType = AuthConstants.ClaimTypes.UserName,
                EnableValidationResultCache = true,
                ValidationResultCacheDuration = TimeSpan.FromMinutes(10)
            });

            GlobalContext.Properties["user"]
                = new HttpContextUserNameProvider();
        }
    }
}