using System.Security.Claims;
using System.Security.Principal;
using AppFramework.Auth;

namespace AssetManager.WebApi.Extensions
{
    public static class IPrincipalExtension
    {
        public static long GetId(this IPrincipal user)
        {
            return long.Parse(
                ((ClaimsIdentity) user.Identity)
                .FindFirst(AuthConstants.ClaimTypes.Subject)
                .Value);
        }
    }
}