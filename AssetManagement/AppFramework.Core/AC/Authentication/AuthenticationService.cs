using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AppFramework.Core.AC.Providers;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DataProxy;

namespace AppFramework.Core.AC.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public long CurrentUserId
        {
            get { return CurrentUser.Asset.ID; }   
        }

        public AssetUser CurrentUser
        {
            get { return _currentUser ?? (_currentUser = _userService.GetCurrentUser()); }
        }

        /// <summary>
        /// Gets the Authentication Data provider
        /// </summary>
        public IAuthenticationStorageProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Gets if authentication data actual or not.
        /// Uses in checks for re-initializing storage data.
        /// </summary>
        public bool IsStorageActual
        {
            get { return _provider.IsActual; }
            set { _provider.IsActual = value; }
        }

        private readonly IAuthenticationStorageProvider _provider;
        private AssetUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        /// <summary>
        /// Class constructor with initialization
        /// </summary>
        public AuthenticationService(
            IUnitOfWork unitOfWork, 
            IUserService userService,
            IAuthenticationStorageProvider storageProvider = null)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            if (userService == null)
                throw new ArgumentNullException("userService");
            _userService = userService;
            if (storageProvider == null)
                storageProvider = new SessionAuthenticationStorageProvider();
            _provider = storageProvider;
            _unitOfWork = unitOfWork;
        }
     
        /// <summary>
        /// Returns the object with user authentication data
        /// </summary>
        /// <returns>AuthenticationStorage object</returns>
        public AuthenticationStorage GetStorage()
        {
            return _provider.GetStorage();
        }

        /// <summary>
        /// Saves user's authentication data
        /// </summary>
        public void SaveStorage()
        {
            _provider.SaveStorage(new AuthenticationStorage());
        }

        /// <summary>
        /// Setting up all permissions for provided user
        /// </summary>
        public void InitializePermissions()
        {
            // get the reference to the storage object
            var authData = _provider.GetStorage();
            // filling the storage
            authData.Users = (from userId in GetUsersTree(CurrentUserId)
                select _userService.GetById(userId)).ToList();

            // writting down filled data
            _provider.SaveStorage(authData);
        }

        /// <summary>
        /// Returns the list of permissions for given asset
        /// </summary>
        /// <param name="asset">Asset to check the permissions</param>
        /// <returns>Permission code</returns>
        public Permission GetPermission(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException("Asset is not provided");

            long? ownerId = null;
            if (asset[AttributeNames.OwnerId] != null && asset[AttributeNames.OwnerId].ValueAsId.HasValue)
                ownerId = asset[AttributeNames.OwnerId].ValueAsId;
            long? userId = null;
            if (asset[AttributeNames.UserId] != null && asset[AttributeNames.UserId].ValueAsId.HasValue)
                userId = asset[AttributeNames.UserId].ValueAsId;
            long? deptId = null;
            if (asset[AttributeNames.DepartmentId] != null && asset[AttributeNames.DepartmentId].ValueAsId.HasValue)
                deptId = asset[AttributeNames.DepartmentId].ValueAsId;
            var taxonomyItems = _unitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(asset.GetConfiguration().ID);
            return GetPermission(asset.ID,                                 
                                 asset.GetConfiguration().ID,
                                 ownerId,
                                 deptId,
                                 (from t in taxonomyItems
                                  select t.TaxonomyItemId).ToList(),
                                  asset.IsUser,
                                  userId);
        }

        /// <summary>
        /// Returns the list of permissions for asset by given asset attributes
        /// </summary>
        /// <param name="dynEntityId">Asset ID</param>
        /// <param name="isUser">Is this asset type of User</param>
        /// <param name="dynEntityConfigId">AssetType ID</param>
        /// <param name="ownerId">Asset's owner ID</param>
        /// <param name="departmentId">Asset's department ID</param>
        /// <param name="taxonomyItemsIds">Asset's TaxonomyItems IDs</param>
        /// <returns>Permission</returns>
        private Permission GetPermission(long dynEntityId,                                         
                                         long dynEntityConfigId,
                                         long? ownerId,
                                         long? departmentId,
                                         List<long> taxonomyItemsIds,
                                         bool isUser,
                                         long? userId = null)
        {
            var result = Permission.DDDD;

            // get the access to the rights storage
            if (!_provider.IsActual)
                InitializePermissions();
            var authData = _provider.GetStorage();

            // if provided asset is user (and child employee), return the rights for him;            
            if (isUser && (authData.Users.Any(u => u.Asset.ID == dynEntityId) || dynEntityId == 0))
            {
                result = CurrentUser.PermissionOnUsers;
            }

            // if provided asset belongs to current user or to his child employee            
            else if ((ownerId.HasValue && authData.Users.Any(u => u.Asset.ID == ownerId.Value))
                     || (userId.HasValue && (authData.Users.Any(u => u.Asset.ID == userId.Value))
                         || CurrentUser.Asset.ID == ownerId
                         || CurrentUser.Asset.ID == userId))
            {
                result = PermissionsProvider.Or(
                    CurrentUser.PermissionOnUsers,
                    CurrentUser.Permissions.GetPermission(dynEntityConfigId, departmentId, taxonomyItemsIds));
            }
            else
            {
                // in other cases look for permission in Rights list           
                result = CurrentUser.Permissions.GetPermission(dynEntityConfigId, departmentId, taxonomyItemsIds);
            }
            return result;
        }

        public bool IsReadingAllowed(AssetType assetType)
        {
            if (assetType == null)
                throw new ArgumentNullException();
            return GetFullPermissions(assetType).CanRead();
        }

        public bool IsWritingAllowed(AssetType assetType)
        {
            if (assetType == null)
                throw new ArgumentNullException();
            var taxonomyItems = _unitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(assetType.ID);
            var taxonomies = taxonomyItems.Select(t => t.TaxonomyItemId).ToList();
            return CurrentUser.Permissions.GetPermission(assetType.ID, null, taxonomies).CanWrite();
        }

        private Permission GetFullPermissions(AssetType assetType)
        {
            var taxonomyItems = _unitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(assetType.ID);
            var taxonomies = taxonomyItems.Select(t => t.TaxonomyItemId).ToList();
            var declaredPermisson = CurrentUser.Permissions.GetPermission(assetType.ID, null, taxonomies);
            var usersPermissions = _provider.GetStorage()
                .Users.Select(u => u.Permissions.GetPermission(assetType.ID, null, taxonomies))
                .ToArray();
            return usersPermissions.Any()
                ? PermissionsProvider.Or(declaredPermisson, PermissionsProvider.Or(usersPermissions))
                : declaredPermisson;
        }

        /// <summary>
        /// Resursively returns the list of all users 
        /// which are belongs to the provided user
        /// </summary>
        /// <param name="userId">ID of user to retrieve child employes</param>
        /// <returns>List of users IDs</returns>
        public List<long> GetUsersTree(long userId)
        {
            var data = _unitOfWork.SqlProvider.ExecuteReader(StoredProcedures.GetUsersTree,
                new SqlParameter[] { 
                        new SqlParameter("@UserId", userId) { SqlDbType = System.Data.SqlDbType.BigInt },
                },
                System.Data.CommandType.StoredProcedure);
            var returnList = new List<long>();
            while (data.Read())
            {
                long id;
                long.TryParse(data[0].ToString(), out id);
                returnList.Add(id);
            }
            data.Close();
            return returnList;
        }

    }

    public static class PermissionExtensions
    {
        public static Permission GetPermission(this List<RightsEntry> rules, long dynEntityConfigId, long? deptId,
            List<long> taxonomyItemsIds)
        {
            // list of all matched rules 
            var matchedRules = rules
                .Where(r => r.Matches(dynEntityConfigId, deptId, taxonomyItemsIds))
                .ToList();

            // list of normal rules
            var allowRules = matchedRules.Where(r => r.IsDeny == false).ToList();

            // list of denying rules (which is for denying)
            var denyRules = matchedRules.Where(r => r.IsDeny).ToList();

            // if there's any deny rules, multiply them with normal,
            // else return just normal
            // add normal rules together,
            // add denying rules together,
            // finally Multiply both results, 
            // because they overrides each other:
            // normal & (~ denying)
            if (denyRules.Any() && allowRules.Any())
                return PermissionsProvider.And(allowRules.Or(), denyRules.Or().Not());
            return allowRules.Any() 
                ? allowRules.Or() 
                : Permission.DDDD;
        }
    }
}
