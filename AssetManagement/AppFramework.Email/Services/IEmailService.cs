namespace AppFramework.Email.Services
{
    public interface IEmailService
    {
        void SendContactFormEmail(string userName, string userEmail, string subject, string message);

        void SendForgotPasswordMail(string emailTo, string username, string password);
    }
}