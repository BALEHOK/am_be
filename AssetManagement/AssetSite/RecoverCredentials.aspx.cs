using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace AssetSite
{
    public partial class RecoverCredentials1 : BasePage
    {
        protected override void InitializeCulture()
        {          
            AssetSite.Helpers.Culture.InitCulture();
            base.InitializeCulture();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PasswordRecoveryForm.SendingMail += new MailMessageEventHandler(PasswordRecoveryForm_SendingMail);
            PasswordRecoveryForm.SendMailError += new SendMailErrorEventHandler(PasswordRecoveryForm_SendMailError);
        }

        protected void PasswordRecovery1_UserLookupError(object sender, EventArgs e)
        {
            UserNotFoundValidator.IsValid = false;
        }

        protected void PasswordRecoveryForm_VerifyingUser(object sender, LoginCancelEventArgs e)
        {
            if (PasswordRecoveryForm.UserName.Contains("@"))
            {
                string userName = System.Web.Security.Membership.GetUserNameByEmail(PasswordRecoveryForm.UserName);

                if (userName != null)
                {
                    PasswordRecoveryForm.UserName = userName;
                }
            }
        }

        private void PasswordRecoveryForm_SendingMail(object sender, MailMessageEventArgs e)
        {
            MembershipUser user = System.Web.Security.Membership.GetUser(PasswordRecoveryForm.UserName);
            if (user != null)
            {
                string newPassword = user.ResetPassword();
                e.Message.Body = e.Message.Body.Replace("{USERNAME}", user.UserName)
                                               .Replace("{PASSWORD}", newPassword);
            }
        }

        private void PasswordRecoveryForm_SendMailError(object sernder, SendMailErrorEventArgs e)
        {
            e.Handled = lblSmtpErrorSettings.Visible = true;
            PasswordRecoveryForm.Visible = false;
        }
    }
}