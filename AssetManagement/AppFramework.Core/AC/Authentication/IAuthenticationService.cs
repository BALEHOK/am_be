using System.Collections.Generic;
using AppFramework.Core.AC.Providers;
using AppFramework.Core.Classes;

namespace AppFramework.Core.AC.Authentication
{
    public interface IAuthenticationService
    {
        long CurrentUserId { get; }
    
        AssetUser CurrentUser { get; }

        /// <summary>
        /// Gets the Authentication Data provider
        /// </summary>
        IAuthenticationStorageProvider Provider { get; }

        /// <summary>
        /// Gets if authentication data actual or not.
        /// Uses in checks for re-initializing storage data.
        /// </summary>
        bool IsStorageActual { get; set; }
      
        /// <summary>
        /// Returns the object with user authentication data
        /// </summary>
        /// <returns>AuthenticationStorage object</returns>
        AuthenticationStorage GetStorage();

        /// <summary>
        /// Saves user's authentication data
        /// </summary>
        /// <param name="storage">AuthenticationStorage object</param>
        void SaveStorage();

        /// <summary>
        /// Setting up all permissions for provided user
        /// </summary>
        /// <param name="user">Currently logged user</param>
        void InitializePermissions();

        bool IsReadingAllowed(AssetType assetType);

        bool IsWritingAllowed(AssetType assetType);

        /// <summary>
        /// Returns the list of permissions for given asset
        /// </summary>
        /// <param name="asset">Asset to check the permissions</param>
        /// <returns>Permission code</returns>
        Permission GetPermission(Asset asset);
    }
}