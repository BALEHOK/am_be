using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace AppFramework.Core.Helpers
{
    public class SmtpSettings
    {

        public static bool SendEmail(string from, string fromName, string to, string body, string subject)
        {
            bool isSend = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from, fromName);
            mailMessage.To.Add(to);
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            SmtpClient client = new SmtpClient();
            try
            {
                client.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                if (ex.StatusCode == SmtpStatusCode.InsufficientStorage)
                {
                    client.Send(mailMessage);
                }
                else
                {
                    isSend = false;
                }

            }
            return isSend;
        }
    }
}
