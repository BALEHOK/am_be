using System.Collections.Generic;

namespace AppFramework.Core.Classes
{
    public interface IUserService
    {
        AssetUser GetUser(string username, bool userIsOnline);
        AssetUser GetCurrentUser();
        void UpdateUser(AssetUser user);
        TaskRightsList GetUserTaskRightsList(long userId);
        IEnumerable<AssetUser> GetAllUsers();

        /// <summary>
        /// Returns the first found user by given name
        /// </summary>
        /// <param name="name">Name for searching</param>
        /// <returns>AssetUser object</returns>
        AssetUser FindByName(string name);

        /// <summary>
        /// Returns the first found user by given email
        /// </summary>
        /// <param name="email">Email for searching</param>
        /// <returns>AssetUser object</returns>
        AssetUser FindByEmail(string email);

        /// <summary>
        /// Returns the user by his unique key
        /// </summary>
        /// <param name="providerUserKey">Asset ID</param>
        /// <returns>AssetUser</returns>
        AssetUser GetById(long userId);
    }
}