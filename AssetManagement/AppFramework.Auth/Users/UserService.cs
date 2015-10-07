using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using AppFramework.Core.Classes;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using Task = System.Threading.Tasks.Task;

namespace AppFramework.Auth.Users
{
    public class UserService : UserServiceBase
    {
        private readonly IUserManager _userManager;

        public UserService(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var username = context.UserName;
            var password = context.Password;

            context.AuthenticateResult = null;

            AssetUser user;
            if (_userManager.ValidateUser(username, password)
                && (user = _userManager.GetUser(username)) != null
                && !user.IsLockedOut)
            {
                var claims = GetClaimsForAuthenticateResult(user);
                context.AuthenticateResult = new AuthenticateResult(user.Id.ToString(), username, claims);
            }
            else
            {
                context.AuthenticateResult = new AuthenticateResult(Constants.TokenErrors.InvalidGrant);
            }

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var requestedClaimTypes = context.RequestedClaimTypes;
            var identity = (ClaimsIdentity) context.Subject.Identity;

            if (requestedClaimTypes != null)
            {
                var claims = requestedClaimTypes
                    .Select(t => GetIdentityClaim(identity, t))
                    .Where(c => c != null);

                context.IssuedClaims = claims;
            }

            return Task.FromResult(0);
        }

        private static Claim GetIdentityClaim(ClaimsIdentity identity, string claimType)
        {
            var claim = identity.FindFirst(claimType);
            if (claim != null)
            {
                return new Claim(claimType, claim.Value);
            }

            return null;
        }

        protected virtual IEnumerable<Claim> GetClaimsForAuthenticateResult(AssetUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(AuthConstants.ClaimTypes.UserName, user.UserName),
                new Claim(AuthConstants.ClaimTypes.Email, user.Email),

                new Claim(AuthConstants.ClaimTypes.LastLogin, user.LastLoginDate.ToString("o")),
                
                new Claim(AuthConstants.ClaimTypes.Role, "user"),
                new Claim(AuthConstants.ClaimTypes.UserRights, user.GetUserRights()),
                new Claim(AuthConstants.ClaimTypes.UserRole, user.Asset["Role"].Value)
            };

            return claims;
        }
    }
}