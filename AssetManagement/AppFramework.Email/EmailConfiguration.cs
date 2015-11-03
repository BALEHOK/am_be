using AppFramework.Email.Services;
using Microsoft.Practices.Unity;

namespace AppFramework.Email
{
    public class EmailConfiguration : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<IEmailService, EmailService>()
                .RegisterType<IViewLoader, ViewLoader>();
        }
    }
}
