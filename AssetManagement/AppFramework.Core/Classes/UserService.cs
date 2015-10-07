using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Web;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;

        public UserService(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
        }

        public AssetUser GetCurrentUser()
        {
            // Bug [Aleksandr Shukletsov] related to AM-329. Thread.CurrentPrincipal is not set to ClaimsPrincipal
            var securityUser = HttpContext.Current != null
                ? HttpContext.Current.User
                : System.Threading.Thread.CurrentPrincipal;

                if (!securityUser.Identity.IsAuthenticated)
                    throw new AuthenticationException("User is not authorized");
            return FindByName(securityUser.Identity.Name);
        }

        public AssetUser GetUser(string username, bool userIsOnline)
        {
            if (string.IsNullOrEmpty(username))
            {
                var securityUser = System.Threading.Thread.CurrentPrincipal;
                if (securityUser.Identity.IsAuthenticated)
                {
                    username = securityUser.Identity.Name;
                }
                else
                {
                    return null;
                }
            }
            var user = FindByName(username);
            return user;
        }
    
        public void UpdateUser(AssetUser user)
        {
            var dtService = new DataTypeService(_unitOfWork);
            var adapter = new DynColumnAdapter(dtService);
            var provider = new DynTableProvider(_unitOfWork, adapter, dtService);
            provider.UpdateAsset(user.ToAsset());
        }

        /// <summary>
        /// Returns the first found user by given name
        /// </summary>
        /// <param name="name">Name for searching</param>
        /// <returns>AssetUser object</returns>
        public AssetUser FindByName(string name)
        {
            var at = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User);
            var asset = _assetsService.GetAssetByParameters(at, new Dictionary<string, string>
            {
                {AttributeNames.ActiveVersion, "true"},
                {AttributeNames.Name, name}
            });
            return asset != null
                ? FromAsset(asset)
                : null;
        }

        /// <summary>
        /// Returns the first found user by given name
        /// </summary>
        /// <param name="name">Name for searching</param>
        /// <returns>AssetUser object</returns>
        public IEnumerable<AssetUser> GetAllUsers()
        {
            var at = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User);
            var assets = _assetsService.GetAssetsByParameters(at, new Dictionary<string, string>
            {
                {AttributeNames.ActiveVersion, "true"},
            });
            return assets.Select(FromAsset);
        }

        /// <summary>
        /// Returns the first found user by given email
        /// </summary>
        /// <param name="email">Email for searching</param>
        /// <returns>AssetUser object</returns>
        public AssetUser FindByEmail(string email)
        {
            var at = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User);
            var asset = _assetsService.GetAssetByParameters(at, new Dictionary<string, string>
            {
                {AttributeNames.ActiveVersion, "true"},
                {AttributeNames.Email, email}
            });
            return asset != null
                ? FromAsset(asset)
                : null;
        }

        /// <summary>
        /// Returns the user by his unique key
        /// </summary>
        /// <param name="providerUserKey">Asset ID</param>
        /// <returns>AssetUser</returns>
        public AssetUser GetById(long userId)
        {
            var assetType = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User);
            var user = _assetsService.GetAssetById(userId, assetType);
            return user != null
                ? FromAsset(user)
                : null;
        }

        public AssetUser FromAsset(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException("Asset");

            DateTime UpdateDate = DateTime.Now;
            DateTime LastLoginDate = DateTime.Now;
            DateTime LastActivityDate = DateTime.Now;
            DateTime LastPasswordChangeDate = DateTime.Now;
            DateTime LastLockoutdate = DateTime.Now;

            DateTime res;
            if (DateTime.TryParse(asset["UpdateDate"].Value, ApplicationSettings.DisplayCultureInfo, DateTimeStyles.None,
                out res))
            {
                UpdateDate = res;
            }

            if (DateTime.TryParse(asset["LastLoginDate"].Value, ApplicationSettings.DisplayCultureInfo,
                DateTimeStyles.None, out res))
            {
                LastLoginDate = res;
            }

            if (DateTime.TryParse(asset["LastActivityDate"].Value, ApplicationSettings.DisplayCultureInfo,
                DateTimeStyles.None, out res))
            {
                LastActivityDate = res;
            }

            if (DateTime.TryParse(asset["LastLockoutDate"].Value, ApplicationSettings.DisplayCultureInfo,
                DateTimeStyles.None, out res))
            {
                LastLockoutdate = res;
            }

            long currentUserId = asset.ID;
            var permissions = (from entry in _unitOfWork.RightsRepository.Get(r => r.UserId == currentUserId)
                select new RightsEntry(entry)).ToList();

            return new AssetUser("DynEntityMembershipProvider", asset[AttributeNames.Name].Value, currentUserId,
                asset[AttributeNames.Email].Value, "", "", true, false,
                UpdateDate, LastLoginDate, LastActivityDate, LastPasswordChangeDate, LastLockoutdate, asset,
                GetUserRoles(currentUserId),
                permissions);
        }

        public IEnumerable<string> GetUserRoles(long userId)
        {
            var roles = _unitOfWork.RoleRepository.Get(r => r.UserInRole.Any(u => u.UserId == userId));
            return roles.Select(r => r.RoleName).ToList();
        }

        /// <summary>
        /// Returns the tasks permissions
        /// </summary>
        /// <returns></returns>
        public TaskRightsList GetUserTaskRightsList(long userId)
        {
            var taskrights = _unitOfWork.TaskRightRepository.Get(r => r.UserId == userId);
            var rList = new TaskRightsList();
            foreach (var entry in taskrights)
            {
                rList.Items.Add(new TaskRightsEntry(entry));
            }
            return rList;
        }
    }
}
