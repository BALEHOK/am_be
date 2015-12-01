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
    using System;
    using AppFramework.ConstantsEnumerators;

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
                    var attributeRepository = new AttributeRepository(UnitOfWork);
                    var assetsService = new AssetsService(UnitOfWork, atRepository, attributeRepository, attributeValueFormatter, rightsService);
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

        [Dependency]
        public IRoleService RoleService
        {
            get
            {
                return _roleService ?? (_roleService = new RoleService());
            }
            set
            {
                if (value != null)
                    _roleService = value;
            }
        }

        private IUserService _userService;
        private IUnitOfWork _unitOfWork;
        private IRoleService _roleService;

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotSupportedException();
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
            throw new NotSupportedException();
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
            throw new NotSupportedException();
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
            // TODO: to implement if really needed. Otherwise use IRolesService.
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured applicationName.
        /// </returns>
        public override string[] GetAllRoles()
        {
            return RoleService.GetAllRoles()
                .Select(r => r.ToString())
                .ToArray();
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
                roleNames = user.Roles.ToArray();
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
            // TODO: to implement if really needed. Otherwise use IRolesService.
            throw new NotSupportedException();
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
            var result = false;
            PredefinedRoles role;
            var user = UserService.FindByName(username);
            if (user != null && Enum.TryParse(roleName, out role))
            {
                var userInRoles = UnitOfWork.UserInRoleRepository
                    .Where(u => u.UserId == user.Asset.ID)
                    .ToList();
                result = userInRoles.Any(ur => ur.RoleId == (int)role);
            }
            return result;
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // TODO: to implement if really needed. Otherwise use IRolesService.
            throw new NotSupportedException();
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
            // TODO: to implement if really needed. Otherwise use IRolesService.
            throw new NotSupportedException();
        }
    }
}