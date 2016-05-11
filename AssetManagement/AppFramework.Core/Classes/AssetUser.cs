using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Security;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// Class derieved from MembershipUser. 
    /// DynEntityUserProvider returns AssetUser instead simple MembershipUser
    /// </summary>
    [Serializable]
    public class AssetUser : MembershipUser
    {
        public long Id { get { return Asset.ID; } }

        public string Email
        {
            get
            {
                return this[AttributeNames.Email].Value;
            }
        }

        public string UserName
        {
            get
            {
                return this[AttributeNames.Name].Value;
            }
        }

        public bool IsAdministrator
        {
            get { return Roles.Contains(PredefinedRoles.Administrators.ToString()); }
        }

        public IEnumerable<string> Roles { get; private set; }

        /// <summary>
        /// Returns the permissions
        /// </summary>
        /// <value></value>
        public List<RightsEntry> Permissions { get; private set; }

        public AssetUser(string providername,
            string username,
            object providerUserKey,
            string email,
            string passwordQuestion,
            string comment,
            bool isApproved,
            bool isLockedOut,
            DateTime creationDate,
            DateTime lastLoginDate,
            DateTime lastActivityDate,
            DateTime lastPasswordChangedDate,
            DateTime lastLockedOutDate,
            Asset asset,
            IEnumerable<string> roles,
            IEnumerable<RightsEntry> permissions) :
                base(providername,
                    username,
                    providerUserKey,
                    email,
                    passwordQuestion,
                    comment,
                    isApproved,
                    isLockedOut,
                    creationDate,
                    lastLoginDate,
                    lastActivityDate,
                    lastPasswordChangedDate,
                    lastLockedOutDate)
        {
            Asset = asset;
            Roles = roles.ToArray();
            Permissions = permissions.ToList();
        }

        /// <summary>
        /// Gets the Asset object on which this user is based
        /// </summary>
        public Asset Asset { get; private set; }

        /// <summary>
        /// Gets the permission on all child users of this user
        /// </summary>
        public Permission PermissionOnUsers
        {
            get
            {
                return PermissionsProvider.GetByCode(
                    Convert.ToByte(Asset["PermissionOnUsers"].Value));
            }
        }

        /// <summary>
        /// Returns the Asset object on which this user is based
        /// </summary>
        /// <returns></returns>
        public Asset ToAsset()
        {
            return Asset;
        }

        public AssetAttribute this[string index]
        {
            get { return this.ToAsset()[index]; }
        }
        
        /// <summary>
        /// Gets or sets the date and time when the membership user was last authenticated or accessed the application.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The date and time when the membership user was last authenticated or accessed the application.
        /// </returns>
        public override DateTime LastActivityDate
        {
            get
            {                
                DateTime val = DateTime.Now;
                if (DateTime.TryParse(Asset["LastActivityDate"].Value, ApplicationSettings.PersistenceCultureInfo, DateTimeStyles.None, out val))
                {
                    return val;
                }
                return DateTime.Now;
            }
            set
            {                
                Asset["LastActivityDate"].Value = value.ToString();
            }
        }

        // todo: should be refactored to RightsEntries
        public string GetUserRights()
        {
            var user = Asset;

            var rightsList = new List<string>
            {
                SecuredModules.ReportsBuilder, 
                SecuredModules.ReplaceFuntion, 
                SecuredModules.InventoryScanner,
                SecuredModules.StockScanner
            };

            var result = new List<string>();
            rightsList.ForEach(name =>
            {
                if (user[name] != null && !string.IsNullOrEmpty(user[name].Value))
                {
                    bool isEnabled;
                    bool.TryParse(user[name].Value, out isEnabled);
                    if (isEnabled)
                        result.Add(name);
                }
            });
            return string.Join(",", result);
        }
        
        /// <summary>
        /// Returns the encrypted password from DB storage for the current user.
        /// </summary>
        /// <returns></returns>
        public override string GetPassword()
        {
            return this.Asset["Password"].Value;
        }

        /// <summary>
        /// Returns user language.
        /// </summary>
        /// <returns></returns>
        public string GetUserLanguage()
        {
            string strculture = string.Empty;
            foreach (var item in this.Asset.Attributes.Where(a => a.IsDynamicList))
            {
                if (item.Value.ToLower().StartsWith("en") || item.Value.ToLower().StartsWith("nl") || item.Value.ToLower().StartsWith("fr"))
                {
                    strculture = item.Value;
                    break;
                }
            }
            return strculture;
        }
    }
}
