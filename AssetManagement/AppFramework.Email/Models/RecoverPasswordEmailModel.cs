namespace AppFramework.Email.Models
{
    public class RecoverPasswordEmailModel
    {
        public string Header { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}