using AppFramework.Core.Classes.Batch;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppFramework.Core.Tests
{


    /// <summary>
    ///This is a test class for BatchJobManagerTest and is intended
    ///to contain all BatchJobManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BatchJobManagerTest
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
        ///A test for DequeueJob
        ///</summary>
        [TestMethod()]
        public void DequeueJobTest()
        {
            AppFramework.Entities.BatchJob actual;
            actual = BatchJobManager.DequeueJob();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for ExecuteJob
        ///</summary>
        [TestMethod()]
        public void ExecuteJobTest()
        {
            AppFramework.Entities.BatchJob job = BatchJobManager.DequeueJob(); // TODO: Initialize to an appropriate value
            if (job != null)
                BatchJobManager.ExecuteJob(job);
        }
    }
}
