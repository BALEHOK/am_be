using System.Linq;
using AppFramework.Auth.Config;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;

namespace AppFramework.Auth.Security
{
    public class CorsPolicyServiceFactory
    {
        public static ICorsPolicyService Get()
        {
            var allowedOrigins = AuthConfiguration.Instance.CorsAllowedOrigins;

            var corsPolicyService = new DefaultCorsPolicyService();

            if (allowedOrigins.Contains("*"))
            {
                corsPolicyService.AllowAll = true;
            }
            else
            {
                corsPolicyService.AllowedOrigins = allowedOrigins;
            }

            return corsPolicyService;
        }
    }
}