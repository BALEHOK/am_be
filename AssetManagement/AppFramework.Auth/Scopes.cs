using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace AppFramework.Auth
{
    public static class Scopes
    {
        private static readonly List<Scope> scopes = new List<Scope>
        {
            StandardScopes.OpenId,
            new Scope
            {
                Name = AuthConstants.Scopes.Profile,
                DisplayName = "Profile information",
                Type = ScopeType.Identity,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(AuthConstants.ClaimTypes.Subject),
                    new ScopeClaim(AuthConstants.ClaimTypes.UserName, true),
                    new ScopeClaim(AuthConstants.ClaimTypes.LastLogin, true),
                    new ScopeClaim(AuthConstants.ClaimTypes.Email, true),
                    new ScopeClaim(AuthConstants.ClaimTypes.UserRole, true),
                    new ScopeClaim(AuthConstants.ClaimTypes.UserRights, true)
                }
            },
            new Scope
            {
                Name = AuthConstants.Scopes.WebApi,
                DisplayName = "Asset manager API",
                Type = ScopeType.Resource,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(AuthConstants.ClaimTypes.Subject),
                    new ScopeClaim(AuthConstants.ClaimTypes.UserName)
                }
            }
        };

        public static List<Scope> Get()
        {
            return scopes;
        }
    }
}