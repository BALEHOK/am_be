using System;
using System.Configuration;
using System.IO;
using System.Web;
using AppFramework.Core.Helpers;

namespace AssetSite
{
    public partial class Contact : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                StreamReader sr = new StreamReader(Server.MapPath("/templates/mailTemplate.htm"));
                string body = sr.ReadToEnd();
                sr.Close();

                body = body.Replace("%EMAIL%", AuthenticationService.CurrentUser.Email)
                           .Replace("%NAME%", AuthenticationService.CurrentUser.UserName)
                           .Replace("%SUBJECT%", txtSubject.Text)
                           .Replace("%MESSAGE%", txtMessage.Text);


                if (SmtpSettings.SendEmail(ConfigurationManager.AppSettings["mailFromAddress"], ConfigurationManager.AppSettings["mailFromName"],
                    ConfigurationManager.AppSettings["contactFormRecipients"], body, "Contact form"))
                {
                    // show 'thank you' page
                    ContactMultiView.ActiveViewIndex = 1;
                }
                else
                {
                    ContactMultiView.ActiveViewIndex = 2;
                }


            }
        }
    }
}