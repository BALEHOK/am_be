using AppFramework.Core.Classes;

namespace AppFramework.Auth.Users
{
    public interface IUserManager
    {
        bool ValidateUser(string username, string password);
        AssetUser GetUser(string username);
    }
}