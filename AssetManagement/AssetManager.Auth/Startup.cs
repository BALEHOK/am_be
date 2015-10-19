using System.Web.Mvc;
using System.Web.Routing;
using AppFramework.Auth;
using AppFramework.Auth.Security;
using AssetManager.Auth;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services.Default;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace AssetManager.Auth
{
    public class Startup
    {
        public const string AuthRoot = "/core";

        public void Configuration(IAppBuilder app)
        {
            log4net.Config.XmlConfigurator.Configure();

            app.Map(AuthRoot, authApp =>
            {
                var factory = Factory.Configure();
                
                // add custom CSS
                var viewOptions = new DefaultViewServiceOptions();
                viewOptions.Stylesheets.Add("/content/css/am-custom.css");
                factory.ConfigureDefaultViewService(viewOptions);

                var options = new IdentityServerOptions
                {
                    SiteName = AuthConstants.SiteName,
                    SigningCertificate = CertificateFactory.Get(),
                    Factory = factory,

                    // ToDo [Alexandr Shukletsov] set SSL required
                    RequireSsl = false,

                    AuthenticationOptions = new AuthenticationOptions
                    {
                        LoginPageLinks = new LoginPageLink[]
                        {
                            new LoginPageLink
                            {
                                Text = "Recover password",
                                Href = "/recoverpassword"
                            }
                        }
                    }
                };

                authApp.UseIdentityServer(options);
            });

            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}