using AppFramework.Core.Classes;
using AppFramework.Core.Services;
using AppFramework.DataProxy;
using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using AppFramework.Core.Classes.SearchEngine;

namespace AppFramework.Core.AC.Authentication
{
    /// <summary>
    /// Manages all operations with user's permissions
    /// 
    /// ATTENTION! This class declared as static which means 
    /// that the lifetime is about endless.
    /// 
    /// Don't keep any data here, use only clear function invokes
    /// to avoid a hard-detectable bugs!
    /// </summary>
    public sealed class AccessManager : IAccessManager
    {

        # region Singleton implementation
        private static readonly AccessManager obj = new AccessManager();

        private AccessManager()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Application[_globalPermissionsContainer] == null)
                {
                    HttpContext.Current.Application[_globalPermissionsContainer] = new List<long>();
                }
                if (HttpContext.Current.Application[_globalAccessContainer] == null)
                {
                    HttpContext.Current.Application[_globalAccessContainer] = new List<long>();
                }
            }
        }

        static AccessManager() { }

        public static AccessManager Instance
        {
            get
            {
                return obj;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// AuthenticationService
        /// </summary>
        public IAuthenticationService AuthenticationService
        {
            get
            {
                if (_authService == null)
                {
                    var unitOfWork = new UnitOfWork();
                    var rightsService = new RightsService(unitOfWork);
                    var atRepository = AssetTypeRepository.Create(unitOfWork);
                    var linkedEntityFinder = new LinkedEntityFinder(unitOfWork);
                    var attributeValueFormatter = new AttributeValueFormatter(linkedEntityFinder);
                    var attributeRepository = new AttributeRepository(unitOfWork);
                    var indexationService = new IndexationService(unitOfWork);
                    var assetsService = new AssetsService(unitOfWork, atRepository, attributeRepository, attributeValueFormatter, rightsService, indexationService);
                    var userService = new UserService(unitOfWork, atRepository, assetsService);
                    _authService = new AuthenticationService(unitOfWork, userService, atRepository);
                }
                return _authService;
            }
        }

        private AuthenticationService _authService;

        private bool IsForcedForLogout
        {
            get
            {
                var userId = (long)AuthenticationService.CurrentUser.ProviderUserKey;
                if (HttpContext.Current != null)
                {
                    return
                        (HttpContext.Current.Application[_globalAccessContainer] as List<long>).Contains(userId);
                }
                return false;
            }
        }

        private bool IsForcedForRightsUpdate
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var userId = (long)AuthenticationService.CurrentUser.ProviderUserKey;
                    return
                        (HttpContext.Current.Application[_globalPermissionsContainer] as List<long>).Contains(
                            userId);
                }
                return false;
            }
        }

        private string _globalPermissionsContainer = "GC_Permissions";
        private string _globalAccessContainer = "GC_Access";

        private enum StorageType
        {
            Permissions,
            Access
        }

        #endregion

        /// <summary>
        /// Clear all personal data for user
        /// </summary>
        public void ClearPersonalData()
        {
            HttpContext.Current.Session["CurrentUser"] = null;
        }
        
        /// <summary>
        /// Check if the rights storage is up-to-date
        /// </summary>
        public bool IsActual
        {
            get
            {
                if (IsForcedForLogout)
                {
                    var userId = (long)AuthenticationService.CurrentUser.ProviderUserKey;
                    RemoveFromGlobalStorage(StorageType.Access, userId);
                    return false;
                }
                return AuthenticationService.IsStorageActual && !IsForcedForRightsUpdate;
            }
            set
            {
                AuthenticationService.IsStorageActual = value;
            }
        }

        /// <summary>
        /// Sets the rights for provided user 
        /// to the provided rights storage
        /// </summary>
        /// <param name="rights"></param>
        /// <param name="userID"></param>
        public void InitRights()
        {
            _authService = null;
            AuthenticationService.InitializePermissions();
            var userId = (long)AuthenticationService.CurrentUser.ProviderUserKey;
            RemoveFromGlobalStorage(StorageType.Permissions, userId);
            AuthenticationService.IsStorageActual = true;
        }

        /// <summary>
        /// Forces user to update his rights.
        /// </summary>
        /// <param name="userId"></param>
        public void ForceRightsUpdate(long userId)
        {
            AddToGlobalStorage(StorageType.Permissions, userId);
        }

        /// <summary>
        /// Forces logout of user with provided ID.
        /// </summary>
        /// <param name="userId"></param>
        public void ForceLogout(long userId)
        {
            AddToGlobalStorage(StorageType.Access, userId);
        }

        /// <summary>
        /// Adds ID of user to specified storage.
        /// </summary>
        /// <param name="st"></param>
        /// <param name="userId"></param>
        private void AddToGlobalStorage(StorageType st, long userId)
        {
            if (HttpContext.Current != null)
            {
                string storageRef = String.Empty;
                if (st == StorageType.Access)
                {
                    storageRef = _globalAccessContainer;
                }
                else if (st == StorageType.Permissions)
                {
                    storageRef = _globalPermissionsContainer;
                }
                HttpContext.Current.Application.Lock();
                List<long> usersIds =
                    HttpContext.Current.Application[storageRef] as List<long>;
                usersIds.Add(userId);
                HttpContext.Current.Application[storageRef] = usersIds;
                HttpContext.Current.Application.UnLock();
            }
        }

        /// <summary>
        /// Removes ID of user from specified storage
        /// </summary>
        /// <param name="st"></param>
        /// <param name="userId"></param>
        private void RemoveFromGlobalStorage(StorageType st, long userId)
        {
            if (HttpContext.Current != null)
            {
                string storageRef = String.Empty;
                if (st == StorageType.Access)
                {
                    storageRef = _globalAccessContainer;
                }
                else if (st == StorageType.Permissions)
                {
                    storageRef = _globalPermissionsContainer;
                }

                HttpContext.Current.Application.Lock();
                List<long> usersIds =
                    HttpContext.Current.Application[storageRef] as List<long>;
                usersIds.Remove(userId);
                HttpContext.Current.Application[storageRef] = usersIds;
                HttpContext.Current.Application.UnLock();
            }
        }        
    }
}
