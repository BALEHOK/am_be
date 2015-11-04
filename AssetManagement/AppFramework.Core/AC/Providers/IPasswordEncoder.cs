namespace AppFramework.Core.AC.Providers
{
    public interface IPasswordEncoder
    {
        string EncodePassword(string password);
    }
}