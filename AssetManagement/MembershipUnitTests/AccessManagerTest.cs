using AppFramework.Core.AC.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppFramework.Core.Classes;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using TypeMock.ArrangeActAssert;
using TypeMock;
using AppFramework.Core.AC.Providers;

namespace MembershipUnitTests
{
    
    
    /// <summary>
    ///This is a test class for AccessManagerTest and is intended
    ///to contain all AccessManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AccessManagerTest
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

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        public void InstanceTest()
        {
            AccessManager actual;
            actual = AccessManager.Instance;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for IsActual
        ///</summary>
        [TestMethod()]
        public void IsActualTest()
        {
            AccessManager_Accessor target = new AccessManager_Accessor(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsActual();
            Assert.AreEqual(expected, actual);            
        }
        
        /// <summary>
        ///A test for InitRights
        ///</summary>
        [TestMethod()]
        public void InitRightsTest()
        {
            AccessManager_Accessor target = new AccessManager_Accessor(); 
           
            Mock mockProvider = MockManager.MockAll<DynEntityMembershipProvider>();
            mockProvider.ExpectGet("PasswordFormat", MembershipPasswordFormat.Clear);
              
            AssetUser currentUser = Membership.GetUser("Alexey") as AssetUser;
            Assert.IsNotNull(currentUser);

            using (RecordExpectations recorder = RecorderManager.StartRecording())
            {
                HttpContext context 
                    = (HttpContext)RecorderManager.CreateMockedObject(typeof(HttpContext));
                target.InitRights(currentUser);
            }       
                        
        }


        /*
        /// <summary>
        ///A test for GetPermission
        ///</summary>
        [TestMethod()]
        public void GetPermissionTest()
        {
            AccessManager_Accessor target = new AccessManager_Accessor(); // TODO: Initialize to an appropriate value
            AssetUser currentUser = null; // TODO: Initialize to an appropriate value
            Asset asset = null; // TODO: Initialize to an appropriate value
            Permission expected = new Permission(); // TODO: Initialize to an appropriate value
            Permission actual;
            actual = target.GetPermission(currentUser, asset);
            Assert.AreEqual(expected, actual);
            
        }*/
    }
}
