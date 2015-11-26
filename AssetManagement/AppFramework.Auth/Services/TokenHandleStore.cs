using System;
using System.Threading.Tasks;
using AppFramework.Auth.Data;
using AppFramework.Auth.Data.Models;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace AppFramework.Auth.Services
{
    /// <summary>
    /// Token handle store
    /// </summary>
    public class TokenHandleStore : BaseTokenStore<Token>, ITokenHandleStore
    {
        public TokenHandleStore(AuthContext context)
            : base(context, TokenType.TokenHandle)
        {
        }

        /// <summary>
        /// Stores the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override Task StoreAsync(string key, Token value)
        {
            var token = new TokenModel
            {
                Key = key,
                SubjectId = value.SubjectId,
                ClientId = value.ClientId,
                JsonCode = ConvertToJson(value),
                Expiry = DateTimeOffset.UtcNow.AddSeconds(value.Lifetime),
                TokenType = TokenType
            };
            
            Context.Tokens.Add(token);

            return Task.FromResult<object>(Context.SaveChanges());
        }
    }
}