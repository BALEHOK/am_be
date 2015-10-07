using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using AppFramework.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppFramework.DataProxy.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DataRepositoryTests
    {
        public DataRepositoryTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GetRolesTest()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            IEnumerable<Role> roles = unitOfWork.RoleRepository.Get();
            Assert.IsTrue(roles.Count() > 0);
        }

        [TestMethod]
        public void RelationsTest()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            Role role = unitOfWork.RoleRepository.Single(r => r.RoleName == "Only Person", include: (r) => r.UserInRole);
            Assert.IsNotNull(role);
            Assert.IsTrue(role.UserInRole.Count > 0);
        }

        [TestMethod]
        public void SqlProvider_ExecureReader_Test()
        {
            var query = string.Format("SELECT * FROM [{0}]", "ADynEntityUser");
            var unitOfWork = new DataProxy.UnitOfWork();
            var reader = unitOfWork.SqlProvider.ExecureReader(query, null);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public void SqlProvider_ExecuteScalar_Test()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            object tmp = unitOfWork.SqlProvider.ExecuteScalar("GetPermittedAssetsCount",
                new SqlParameter[] { new SqlParameter("@assetTypeId", 89), new SqlParameter("@userId", 1) },
                System.Data.CommandType.StoredProcedure);
            Assert.IsNotNull(tmp);
        }

        [TestMethod]
        public void SqlProvider_ExecuteNonQuery_Test()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            unitOfWork.SqlProvider.ExecuteNonQuery("SELECT 1 FROM DynEntityConfig",
                null,
                System.Data.CommandType.Text);
        }

        [TestMethod]
        public void EntityProvider_Query_Test()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var data = unitOfWork.EntityProvider.Query<Entities.DynEntityIndex>("DynEntityIndex_GetPermitted",
                new ObjectParameter[] { 
							new ObjectParameter("assetTypeId", 89),
							new ObjectParameter("userId",  1),
						  }, System.Data.CommandType.StoredProcedure);
            Assert.IsTrue(data.Count() > 0);
        }

        [TestMethod]
        public void EntityProvider_ExecureReader_Test()
        {
            var query = string.Format("SELECT VALUE a1 FROM DataEntities.DynEntityConfig as a1");
            var unitOfWork = new DataProxy.UnitOfWork();
            var reader = unitOfWork.EntityProvider.ExecureReader(query, null);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public void EntityProvider_ExecuteScalar_Test()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            object tmp = unitOfWork.EntityProvider.ExecuteScalar("DataEntities.GetPermittedAssetsCount",
                new EntityParameter[] { new EntityParameter() { ParameterName = "assetTypeId", Value = 89 }, 
					new EntityParameter(){ParameterName ="userId", Value = 1 } },
                System.Data.CommandType.StoredProcedure);
            Assert.IsNotNull(tmp);
        }

        [TestMethod]
        public void DynEntityAttribConfigInsertTest()
        {
            //var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            var config = new Entities.DynEntityConfig()
            {
                Revision = 33,
                ActiveVersion = true,
                Name = "test",
                DBTableName = "test",
                TypeId = 1,
                ScreenLayoutId = 1,
                Active = true,
                IsSearchable = false,
                IsIndexed = false,
                IsContextIndexed = false,
                UpdateUserId = 777,
                UpdateDate = DateTime.Now,
                IsInStock = false,
                IsUnpublished = false,
                AllowBorrow = false,
                AutoGenerateName = 0
            };

            //var attribute = new Entities.DynEntityAttribConfig()
            //{
            //    DynEntityConfigUid = 89,
            //    Name = "test",
            //    DataTypeUid = 35,
            //    DBTableFieldname = "test",
            //    IsDynListValue = false,
            //    IsFinancialInfo = false,
            //    IsRequired = false,
            //    IsKeyword = false,
            //    IsFullTextInidex = false,
            //    DisplayOnResultList = false,
            //    DisplayOnExtResultList = false,
            //    UpdateUserId = 77,
            //    UpdateDate = DateTime.Now,
            //    Label = "test",
            //    Revision = 1,
            //    Active = true,
            //    ActiveVersion = true,
            //    IsDescription = false,
            //    IsShownInGrid = false,
            //    IsShownOnPanel = false,
            //    AllowEditConfig = false,
            //    AllowEditValue = false,
            //    IsUsedForNames = false
            //};

            //config.DynEntityAttribConfig.Add(attribute);

            using (var u = new DataProxy.UnitOfWork())
            {
                u.RunInTransaction(() =>
                {
                    using (var unitOfWork = new DataProxy.UnitOfWork())
                    {
                        unitOfWork.DynEntityConfigRepository.Insert(config);
                        unitOfWork.Commit();
                        //}
                        //// trying to update detached entity
                        //using (var unitOfWork = new DataProxy.UnitOfWork())
                        //{
                        config.ActiveVersion = false;
                        unitOfWork.DynEntityConfigRepository.Update(config);
                        unitOfWork.Commit();
                    }
                });
            }
        }
    }
}
