using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;

namespace AppFramework.Auth.Security
{
    public class RedirectUriValidator : DefaultRedirectUriValidator
    {
        public override Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            return client.RedirectUris.Contains("*")
                ? Task.FromResult(true)
                : base.IsRedirectUriValidAsync(requestedUri, client);
        }

        public override Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return client.PostLogoutRedirectUris.Contains("*")
                ? Task.FromResult(true)
                : base.IsPostLogoutRedirectUriValidAsync(requestedUri, client);
        }
    }
}