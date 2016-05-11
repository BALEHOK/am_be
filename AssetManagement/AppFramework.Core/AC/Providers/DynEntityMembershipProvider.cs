using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data.SqlClient;
using System.Text;
using System.Web.Security;
using AppFramework.Core.Classes.SearchEngine;
using Microsoft.Practices.Unity;
using AppFramework.Core.Services;

namespace AppFramework.Core.AC.Providers
{
    /// <summary>
    /// Membership provider for accessing to ADynEntityUser table as membership info storage
    /// </summary>
    public class DynEntityMembershipProvider : MembershipProvider, IPasswordEncoder
    {
        [Dependency]
        public IUnitOfWork UnitOfWork
        {
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
                    var indexationService = new IndexationService(UnitOfWork);
                    var assetsService = new AssetsService(UnitOfWork, atRepository, attributeRepository, attributeValueFormatter, rightsService, indexationService);
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

        private MembershipPasswordFormat _passwordFormat;

        public override string ApplicationName
        {
            get
            {
                return "AssetManagement";
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = UserService.FindByName(username);
            if (user == null)
                throw new ArgumentException();
            bool res = false;
            var pw = new PasswordProvider();
            if (user.GetPassword() == pw.Encrypt(oldPassword))
            {
                user.Asset["Password"].Value = pw.Encrypt(newPassword);
                user.LastActivityDate = DateTime.Now;
                UserService.UpdateUser(user);
                res = true;
            }
            return res;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return true; }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The name of the provider is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The name of the provider has a length of zero.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.
        /// </exception>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the collection of all users where user names are matched with provided value
        /// </summary>
        /// <param name="usernameToMatch">String value to perform the search</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
            //MembershipUserCollection users = new MembershipUserCollection();
            //AssetType at = AssetType.GetByID(PredefinedAttribute.GetDynEntityConfigId(PredefinedEntity.User));

            //List<ISearchCondition> conditions = new List<ISearchCondition>();
            //conditions.Add(new SearchCondition("Name", string.Format("{0}%", usernameToMatch), searchOperator: SearchOperator.Like));
            //conditions.Add(new SearchCondition("ActiveVersion", "True"));

            //var unitOfWork = new DataProxy.UnitOfWork();
            //unitOfWork.DynEntityUserRepository.Where(d => d.Name.StartsWith(usernameToMatch) && d.ActiveVersion == true);
            ////foreach (var item in SearchEngine.FindByAssetType(at.UID, conditions, out totalRecords, pageIndex, pageSize, false))
            ////{
            ////    users.Add(AssetUser.FromAsset(item));
            ////}
            //return users;
        }
        
        public override int GetNumberOfUsersOnline()
        {
            int count = 0;
            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);
            var unitOfWork = new DataProxy.UnitOfWork();
            object tmp = unitOfWork.SqlProvider.ExecuteScalar("select dbo.GetNumberOfUsersOnline(@CompareDate)",
                            new SqlParameter[] { new SqlParameter("@CompareDate", compareTime) });
            if (tmp != null)
                count = (int)tmp;
            return count;
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return UserService.GetUser(username, userIsOnline);
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var user = UserService.GetById(long.Parse(providerUserKey.ToString()));
            if (user == null || !userIsOnline) 
                return user;
            var lastActivity = user.LastActivityDate;
            if (DateTime.Now > lastActivity.AddMinutes(10))
            {
                user.LastActivityDate = DateTime.Now;
                UserService.UpdateUser(user);
            }
            return user;
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        public override string GetUserNameByEmail(string email)
        {
            var user = UserService.FindByEmail(email);
            return user != null ? user.UserName : string.Empty;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            var newPassword = Membership.GeneratePassword(8, 0);
            var pp = new PasswordProvider();
            var user = UserService.FindByName(username);
            user["Password"].Value = pp.Encrypt(newPassword);
            UserService.UpdateUser(user);
            return newPassword;
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            var id = (long)user.ProviderUserKey;
            var userEntity = UserService.GetById(id);
            if (userEntity != null)
            {
                var userAsset = userEntity.Asset;
                userAsset["Email"].Value = user.Email;
                userAsset["Comment"].Value = user.Comment;
                userAsset["IsApproved"].Value = user.IsApproved.ToString();
                userAsset["Username"].Value = user.UserName;
                UserService.UpdateUser(userEntity);
            }
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        public override bool ValidateUser(string username, string password)
        {
            var isValid = false;
            var assetUser = UserService.FindByName(username);
            var userPassword = string.Empty;
            if (assetUser != null)
            {
                userPassword = assetUser["Password"].Value;
                isValid = CheckPassword(password, userPassword);    // compare stored password with given                
            }

            if (isValid)
            {
                var asset = assetUser.ToAsset();
                UnitOfWork.SqlProvider.ExecuteNonQuery(
                    string.Format(
                        "UPDATE [{0}] SET LastLoginDate=@loginDate, LastActivityDate=@activityDate WHERE DynEntityUid=@uid",
                        asset.GetConfiguration().DBTableName),
                    new SqlParameter[]
                    {
                        new SqlParameter("@loginDate", DateTime.Now),
                        new SqlParameter("@activityDate", DateTime.Now),
                        new SqlParameter("@uid", asset.UID),
                    });
                UnitOfWork.Commit();
            }

            return isValid;
        }

        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.
        //
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }
            return pass1 == pass2;
        }

        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //

        public string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var passwordProvider = new PasswordProvider();
                    encodedPassword = passwordProvider.Encrypt(password);
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = UserService.GetAllUsers().ToList();
            totalRecords = users.Count;
            var collection = new MembershipUserCollection();
            users.ForEach(collection.Add);
            return collection;
        }
    }
}
