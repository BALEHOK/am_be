using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Security;
using System;

using AppFramework.Core.Classes;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Providers;

using TypeMock.ArrangeActAssert;
using TypeMock;

namespace MembershipUnitTests
{
    
    
    /// <summary>
    ///This is a test class for DynEntityMembershipProviderTest and is intended
    ///to contain all DynEntityMembershipProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DynEntityMembershipProviderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [ClassInitialize()]
        public static void DynEntityMembershipProviderTestInitialize(TestContext testContext)
        {
            long uid = AssetType.GetByID(PredefinedAttribute.GetDynEntityConfigId("User")).UID;// get UID for last revision of User asset
            AssetUser user = AssetUser.FromAsset(AssetFactory.CreateAsset(uid));
            AssetUser user2 = AssetUser.FromAsset(AssetFactory.CreateAsset(uid));

            // we need just this attributes
            user["DynEntityUid"].Value = "2";
            user["Name"].Value = "Marco";
            user["Password"].Value = "123";
            user["Email"].Value = "marco@example.com";
            user["LastLoginDate"].Value = DateTime.Now.ToString();
            user["LastActivityDate"].Value = DateTime.Now.ToString();
            user["LastLockoutDate"].Value = DateTime.Now.ToString();
            user["ParentUser"].Value = "1";

            user["DynEntityUid"].Value = "1";
            user2["Name"].Value = "Alexey";
            user2["Password"].Value = "qByEtj60v3Mu0yhnke5tp1whJl0="; // hashed value of "123" string
            user2["Email"].Value = "alexey@example.com";
            user2["DynEntityUid"].Value = 1.ToString();
            user2["LastLoginDate"].Value = DateTime.Now.ToString();
            user2["LastActivityDate"].Value = DateTime.Now.ToString();
            user2["LastLockoutDate"].Value = DateTime.Now.ToString();
            user2["ParentUser"].Value = "0";

            

            //Mock mock = MockManager.MockAll<AssetUser>(Constructor.NotMocked);            
            //mock.ExpectAndReturn("GetByName", user).When("Marco"); // for every get user with name "Marco" return our Marco user
            //mock.ExpectAndReturn("GetByName", user2).When("Alexey"); // same for Alexey user
        }

        /// <summary>
        ///A test for ValidateUser
        ///</summary>
        [TestMethod()]
        [ClearMocks]
        public void ValidateUserTest()
        {
            Mock mockProvider = MockManager.MockAll<DynEntityMembershipProvider>();
            mockProvider.ExpectGet("PasswordFormat", MembershipPasswordFormat.Clear);   // for every DynEntityMembershipProvider PasswordFormat property returns Clear value
                       
            Assert.AreEqual(true, Membership.ValidateUser("Marco", "123"), "Clear");
            Assert.AreEqual(false, Membership.ValidateUser("Marco", "asdas"), "Clear");

            // hashed password            
            mockProvider.ExpectGet("PasswordFormat", MembershipPasswordFormat.Hashed);  // now, test same for hashed password
                        
            Assert.AreEqual(true, Membership.ValidateUser("Alexey", "123"), "Hashed");
            Assert.AreEqual(false, Membership.ValidateUser("Alexey", "asdas"), "Hashed");
        }
             
        /// <summary>
        ///A test for GetAllUsers
        ///</summary>
        [TestMethod()]
        public void GetAllUsersTest()
        {
            MembershipUserCollection col1 = Membership.GetAllUsers();
            Assert.AreEqual(2, col1.Count);
            Assert.AreEqual(col1["Marco"].ProviderUserKey, "2");
            Assert.AreEqual(col1["Alexey"].ProviderUserKey, "1");

            int cnt = 0;
            col1 = Membership.GetAllUsers(0,1,out cnt);
            Assert.AreEqual(1, cnt);
            Assert.AreEqual(col1["Alexey"].ProviderUserKey, "1");

            cnt = 0;
            col1 = Membership.GetAllUsers(1, 1, out cnt);
            Assert.AreEqual(1, cnt);
            Assert.AreEqual(col1["Marco"].ProviderUserKey, "2");

            cnt = 0;
            col1 = Membership.GetAllUsers(0, 10, out cnt);
            Assert.AreEqual(2, cnt);

            cnt = 0;
            col1 = Membership.GetAllUsers(100, 200, out cnt);
            Assert.AreEqual(0, cnt);
        }

        /// <summary>
        ///A test for GetNumberOfUsersOnline
        ///</summary>
        [TestMethod()]
        [ClearMocks]
        public void GetNumberOfUsersOnlineTest()
        {
            Mock mock = MockManager.Mock(typeof(System.Web.Security.Membership), Constructor.NotMocked);
            mock.ExpectGet("UserIsOnlineTimeWindow", 0);

            Assert.AreEqual(0, Membership.GetNumberOfUsersOnline());

            mock.ExpectGet("UserIsOnlineTimeWindow", 60);

            Assert.AreNotEqual(0, Membership.GetNumberOfUsersOnline());
        }

        /// <summary>
        ///A test for GetUser
        ///</summary>
        [TestMethod()]        
        [ClearMocks]
        public void GetUserTest1()
        {
            Assert.AreEqual(2.ToString(), Membership.GetUser("Marco").ProviderUserKey.ToString());
            Assert.AreEqual(1.ToString(), Membership.GetUser("Alexey").ProviderUserKey.ToString());
            Assert.IsNull(Membership.GetUser("NoSuchUser"));
        }

        /// <summary>
        ///A test for GetUser
        ///</summary>
        [TestMethod()]
        [ClearMocks]
        public void GetUserTest()
        {
            //Mock mockAsset = MockManager.MockAll<Asset>(Constructor.NotMocked);
            //mockAsset.ExpectCall("UpdateUser");

            long userKey = 2;            
            MembershipUser actual;

            actual = Membership.GetUser(userKey);
            Assert.AreEqual("Marco", actual.UserName);
        }

        /// <summary>
        ///A test for GetUserNameByEmail
        ///</summary>
        [TestMethod()]        
        public void GetUserNameByEmailTest()
        {            
            string email = "marco@example.com";
            string expected = "Marco";

            string actual;
            actual = Membership.GetUserNameByEmail(email);  // existing email
            Assert.AreEqual(actual, expected);

            actual = Membership.GetUserNameByEmail("notexist@example.com"); // not existing email
            Assert.AreEqual(actual, string.Empty);            
        }
    }
}
