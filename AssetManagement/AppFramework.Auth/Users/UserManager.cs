using System.Web.Security;
using AppFramework.Core.Classes;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Auth.Users
{
    public class UserManager : IUserManager
    {
        public bool ValidateUser(string username, string password)
        {
            return Membership.ValidateUser(username, password) &&
                   !Roles.IsUserInRole(username.ToLower(), PredefinedRoles.OnlyPerson.ToString());
        }

        public AssetUser GetUser(string username)
        {
            return Membership.GetUser(username) as AssetUser;
        }
    }
}