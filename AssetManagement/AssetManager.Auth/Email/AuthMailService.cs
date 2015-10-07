using System.Configuration;
using AppFramework.Core.Helpers;
using AssetManager.Auth.Email.Models;

namespace AssetManager.Auth.Email
{
    public class AuthMailService : IMailService
    {
        private const string MailSubject = "Asset manager: ";

        private readonly IViewLoader _viewLoader;

        public AuthMailService()
        {
            _viewLoader = new ViewLoader();
        }

        public bool SendForgotPasswordMail(string emailTo, string username, string password)
        {
            const string header = "Credentials recovery";
            const string subject = MailSubject + header;
            var model = new RecoverPasswordEmailModel
            {
                Header = header,
                Username = username,
                Password = password
            };

            var body = _viewLoader.RenderViewToString("~/Email/Templates/RecoverPasswordEmail.cshtml", model);

            return SmtpSettings.SendEmail(
                ConfigurationManager.AppSettings["mailFromAddress"],
                ConfigurationManager.AppSettings["mailFromName"],
                emailTo, body, subject);
        }
    }
}