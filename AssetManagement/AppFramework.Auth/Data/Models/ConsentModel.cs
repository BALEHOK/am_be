namespace AppFramework.Auth.Data.Models
{
    public class ConsentModel
    {
        public virtual string Subject { get; set; }
        public virtual string ClientId { get; set; }
        public virtual string Scopes { get; set; }
    }
}