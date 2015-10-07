using AppFramework.Auth.Security;
using AppFramework.Auth.Users;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;

namespace AppFramework.Auth
{
    public class Factory
    {
        public static IdentityServerServiceFactory Configure()
        {
            var factory = new IdentityServerServiceFactory();

            var scopeStore = new InMemoryScopeStore(Scopes.Get());
            factory.ScopeStore = new Registration<IScopeStore>(scopeStore);
            var clientStore = new InMemoryClientStore(Clients.Get());
            factory.ClientStore = new Registration<IClientStore>(clientStore);

            factory.UserService = new Registration<IUserService, UserService>();
            factory.Register(new Registration<IUserManager, UserManager>());

            factory.CorsPolicyService = new Registration<ICorsPolicyService>(CorsPolicyServiceFactory.Get());

            factory.RedirectUriValidator = new Registration<IRedirectUriValidator>(new RedirectUriValidator());
            
            return factory;
        }
    }
}