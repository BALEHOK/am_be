using AppFramework.Email.Models;
using Common.Logging;
using System;
using System.Configuration;
using System.Net.Mail;

namespace AppFramework.Email.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _contactFormRecipients;
        private readonly string _fromAddress;
        private readonly string _fromName;
        private ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly IViewLoader _viewLoader;

        public EmailService(IViewLoader viewLoader)
        {
            if (viewLoader == null)
                throw new ArgumentNullException("viewLoader");
            _viewLoader = viewLoader;

            _fromAddress = ConfigurationManager.AppSettings["mailFromAddress"];
            _fromName = ConfigurationManager.AppSettings["mailFromName"];
            _contactFormRecipients = ConfigurationManager.AppSettings["contactFormRecipients"];
        }

        public void SendContactFormEmail(string userName, string userEmail, string subject, string message)
        {
            var model = new ContactFormEmailModel
            {
                UserName = userName,
                UserEmail = userEmail,
                Subject = string.Format("Message from contact form: {0}", subject),
                Message = message
            };

            var body = _viewLoader.RenderViewToString(TemplateNames.ContactFormEmail, model);

            SendEmail(
                _fromAddress,
                _fromName,
                _contactFormRecipients, 
                body,
                model.Subject);
        }

        public void SendForgotPasswordMail(string emailTo, string username, string password)
        {
            const string MailSubject = "Asset manager: ";
            const string header = "Credentials recovery";
            const string subject = MailSubject + header;
            var model = new RecoverPasswordEmailModel
            {
                Header = header,
                Username = username,
                Password = password
            };

            var body = _viewLoader.RenderViewToString(TemplateNames.RecoverPasswordEmail, model);

            SendEmail(
                _fromAddress,
                _fromName,
                emailTo, body, subject);
        }

        private void SendEmail(string from, string fromName, string to, string body, string subject)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(from, fromName),
                IsBodyHtml = true,
                Priority = MailPriority.Normal,
                Subject = subject,
                Body = body,
            };
            mailMessage.To.Add(to);

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Send(mailMessage);
                    _logger.InfoFormat("Email \"{0}\", sent to {1}, from {2}",
                        subject, to, from);
                }
                catch (SmtpException ex)
                {
                    _logger.Error(
                        string.Format("Failed to send email \"{0}\" to {1} from {2}",
                            subject, to, from),
                        ex);
                    throw;
                }
            }
        }
    }
}
