namespace AssetManager.Auth.Email
{
    public interface IMailService
    {
        bool SendForgotPasswordMail(string emailTo, string username, string password);
    }
}