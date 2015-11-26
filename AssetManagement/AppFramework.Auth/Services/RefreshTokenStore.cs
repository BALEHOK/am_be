using System.Threading.Tasks;
using AppFramework.Auth.Data;
using AppFramework.Auth.Data.Models;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace AppFramework.Auth.Services
{
    public class RefreshTokenStore : BaseTokenStore<RefreshToken>, IRefreshTokenStore
    {
        public RefreshTokenStore(AuthContext context)
            : base(context, TokenType.RefreshToken)
        {
        }

        public override Task StoreAsync(string key, RefreshToken value)
        {
            var token = Context.Tokens.Find(key, TokenType);
            if (token == null)
            {
                token = new TokenModel
                {
                    Key = key,
                    SubjectId = value.SubjectId,
                    ClientId = value.ClientId,
                    JsonCode = ConvertToJson(value),
                    TokenType = TokenType
                };
                Context.Tokens.Add(token);
            }

            token.Expiry = value.CreationTime.AddSeconds(value.LifeTime);
            return Task.FromResult(Context.SaveChanges());
        }
    }
}
