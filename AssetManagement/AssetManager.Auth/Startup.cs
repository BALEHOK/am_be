using System.Web.Mvc;
using System.Web.Routing;
using AppFramework.Auth;
using AppFramework.Auth.Security;
using AssetManager.Auth;
using IdentityServer3.Core.Configuration;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace AssetManager.Auth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            log4net.Config.XmlConfigurator.Configure();

            app.Map("/core", authApp =>
            {
                var factory = Factory.Configure();

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