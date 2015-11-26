using System;
using System.Threading.Tasks;
using AppFramework.Auth.Data;
using AppFramework.Auth.Data.Models;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace AppFramework.Auth.Services
{
    public class AuthorizationCodeStore : BaseTokenStore<AuthorizationCode>, IAuthorizationCodeStore
    {
        public AuthorizationCodeStore(AuthContext context)
            : base(context, TokenType.AuthorizationCode)
        {
        }

        public override Task StoreAsync(string key, AuthorizationCode code)
        {
            var efCode = new TokenModel
            {
                Key = key,
                SubjectId = code.SubjectId,
                ClientId = code.ClientId,
                JsonCode = ConvertToJson(code),
                Expiry = DateTimeOffset.UtcNow.AddSeconds(code.Client.AuthorizationCodeLifetime),
                TokenType = TokenType
            };

            Context.Tokens.Add(efCode);
            return Task.FromResult<object>(Context.SaveChanges());
        }
    }
}