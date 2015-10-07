using AppFramework.Core.AC.Authentication;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.AC.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Security;
    using AppFramework.Core.Classes;
    using AppFramework.DataProxy;
    using AppFramework.Entities;
    using Services;

    /// <summary>
    /// Role provider based on DynEntityMembershipProvider
    /// </summary>
    public class DynEntityRoleProvider : RoleProvider
    {
        [Dependency]
        public IUnitOfWork UnitOfWork {
            get
            {
                return _unitOfWork ?? (_unitOfWork = new UnitOfWork());
            }
            set
            {
                if (value != null)
                    _unitOfWork = value;            
            } 
        }

        [Dependency]
        public IUserService UserService
        {
            get
            {
                if (_userService == null)
                {
                    var atRepository = AssetTypeRepository.Create(UnitOfWork);
                    var linkedEntityFinder = new LinkedEntityFinder(UnitOfWork);
                    var attributeValueFormatter = new AttributeValueFormatter(linkedEntityFinder);
                    var rightsService = new RightsService(UnitOfWork);
                    var assetsService = new AssetsService(UnitOfWork, atRepository, attributeValueFormatter, rightsService);
                    _userService = new UserService(UnitOfWork, atRepository, assetsService);
                }
                return _userService;
            }
            set
            {
                if (value != null)
                    _userService = value;
            }
        }

        private IUserService _userService;
        private IUnitOfWork _unitOfWork;

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            var roles = UnitOfWork.RoleRepository.Get(r => roleNames.Contains(r.RoleName));
            foreach (string userName in usernames)
            {
                var user = UserService.FindByName(userName);
                if (user == null)
                    continue;

                foreach (var role in roles)
                {
                    if (!IsUserInRole(user.UserName, role.RoleName))
                    {
                        UnitOfWork.UserInRoleRepository.Insert(new UserInRole() { RoleId = role.RoleId, UserId = user.Asset.ID });
                    }
                }
            }
            UnitOfWork.Commit();
        }

        /// <summary>
        /// Gets or sets the name of the application to store and retrieve role information for.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the application to store and retrieve role information for.
        /// </returns>
        public override string ApplicationName
        {
            get
            {
                return "AssetManagement";
            }
            set { }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            if (!RoleExists(roleName))
            {
                var unitOfWork = new UnitOfWork();
                unitOfWork.RoleRepository.Insert(new Role() { RoleName = roleName });
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if <paramref name="roleName"/> has one or more members and do not delete <paramref name="roleName"/>.</param>
        /// <returns>
        /// true if the role was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var unitOfWork = new UnitOfWork();
            var roleToDelete = unitOfWork.RoleRepository.SingleOrDefault(r => r.RoleName == roleName);
            if (roleToDelete == null)
            {
                return false;
            }
            else
            {
                unitOfWork.RoleRepository.Delete(roleToDelete);
                unitOfWork.Commit();
                return true;
            }
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <returns>
        /// A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch"/> and the user is a member of the specified role.
        /// </returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return GetUsersInRole(roleName).Where(u => u.Contains(usernameToMatch)).ToArray();
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured applicationName.
        /// </returns>
        public override string[] GetAllRoles()
        {
            var unitOfWork = new UnitOfWork();
            string[] roles = unitOfWork.RoleRepository.Get().Select(r => r.RoleName).ToArray();
            return roles;
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
        /// </returns>
        public override string[] GetRolesForUser(string username)
        {
            string[] roleNames = new string[] { };
            var user = UserService.FindByName(username);
            if (user != null)
            {
                var roles = UnitOfWork.RoleRepository.Get(r => r.UserInRole.Any(u => u.UserId == user.Asset.ID));
                roleNames = roles.Select(r => r.RoleName).ToArray();
            }
            return roleNames;
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>
        /// A string array containing the names of all the users who are members of the specified role for the configured applicationName.
        /// </returns>
        public override string[] GetUsersInRole(string roleName)
        {
            List<string> users = new List<string>();
            var role = UnitOfWork.RoleRepository.Get(r => r.RoleName == roleName).SingleOrDefault();
            if (role != null)
            {
                foreach (var ur in role.UserInRole)
                {
                    var au = UserService.GetById(ur.UserId);
                    if (au != null)
                    {
                        users.Add(au.UserName);
                    }
                }
            }
            return users.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>
        /// true if the specified user is in the specified role for the configured applicationName; otherwise, false.
        /// </returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            return GetUsersInRole(roleName).Contains(username);
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var roles = UnitOfWork.RoleRepository.Get(r => roleNames.Contains(r.RoleName));
            foreach (string userName in usernames)
            {
                AssetUser user = UserService.FindByName(userName);
                foreach (string roleName in roleNames)
                {
                    var role = roles.Single(r => r.RoleName == roleName);
                    var userInRole = role.UserInRole.SingleOrDefault(u => u.UserId == user.Asset.ID);
                    if (userInRole != null)
                    {
                        UnitOfWork.UserInRoleRepository.Delete(userInRole);
                    }
                }
            }
            UnitOfWork.Commit();
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        /// <returns>
        /// true if the role name already exists in the data source for the configured applicationName; otherwise, false.
        /// </returns>
        public override bool RoleExists(string roleName)
        {
            var unitOfWork = new UnitOfWork();
            return unitOfWork.RoleRepository.Get(r => r.RoleName == roleName).Count() == 1;
        }
    }
}