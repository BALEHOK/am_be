using AppFramework.Auth.Data;
using AppFramework.Auth.Security;
using AppFramework.Auth.Services;
using AppFramework.Auth.Users;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;

namespace AppFramework.Auth
{
    public class Factory
    {
        public static IdentityServerServiceFactory Configure(string connectionString)
        {
            var factory = new IdentityServerServiceFactory();

            var scopeStore = new InMemoryScopeStore(Scopes.Get());
            factory.ScopeStore = new Registration<IScopeStore>(scopeStore);
            var clientStore = new InMemoryClientStore(Clients.Get());
            factory.ClientStore = new Registration<IClientStore>(clientStore);

            factory.UserService = new Registration<IUserService, UserService>();
            factory.Register(new Registration<IUserManager, UserManager>());

            factory.Register(new Registration<AuthContext>(r => new AuthContext(connectionString)));
            factory.TokenHandleStore = new Registration<ITokenHandleStore, TokenHandleStore>();

            factory.CorsPolicyService = new Registration<ICorsPolicyService>(CorsPolicyServiceFactory.Get());

            factory.RedirectUriValidator = new Registration<IRedirectUriValidator>(new RedirectUriValidator());

            return factory;
        }
    }
}