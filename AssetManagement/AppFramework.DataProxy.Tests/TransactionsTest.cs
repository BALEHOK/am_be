using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppFramework.DataProxy.Tests
{
    [TestClass]
    public class TransactionsTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DistributedTransactionsTest()
        {
            var st = new Entities.SearchTracking()
            {
                Parameters = @"<parametersSetOne></parametersSetOne>",
                ResultCount = 0,
                SearchType = 0,
                UpdateUser = 1,
                UpdateDate = DateTime.Now
            };

            var st2 = new Entities.SearchTracking()
            {
                Parameters = @"<parametersSetTwo></parametersSetTwo>",
                ResultCount = 0,
                SearchType = 0,
                UpdateUser = 1,
                UpdateDate = DateTime.Now
            };

            var unitOfWork = new DataProxy.UnitOfWork();
            var initialCount = unitOfWork.SearchTrackingRepository.Get().Count();
            unitOfWork.RunInTransaction(() =>
            {
                unitOfWork.SearchTrackingRepository.Insert(st);
                unitOfWork.Commit();

                var unitOfWork2 = new DataProxy.UnitOfWork();
                unitOfWork2.SearchTrackingRepository.Insert(st2);
                unitOfWork2.Commit();
                throw new InvalidOperationException();
            });
            Assert.Equals(initialCount, unitOfWork.SearchTrackingRepository.Get().Count());
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DirectSqlWithinTransactionTest()
        {
            var st = new Entities.SearchTracking()
            {
                Parameters = @"<parametersSetOne></parametersSetOne>",
                ResultCount = 0,
                SearchType = 0,
                UpdateUser = 1,
                UpdateDate = DateTime.Now
            };

            var unitOfWork = new DataProxy.UnitOfWork();
            var initialCount = unitOfWork.SearchTrackingRepository.Get().Count();
            unitOfWork.RunInTransaction(() =>
            {
                unitOfWork.SqlProvider.ExecuteNonQuery("INSERT INTO SearchTracking (Parameters,ResultCount,SearchType,UpdateUser,UpdateDate) " +
                    "VALUES(@Parameters, @ResultCount, @SearchType, @UpdateUser, @UpdateDate)",
                    new SqlParameter[] { 
                        new SqlParameter("@Parameters", "<parameters/>"),
                        new SqlParameter("@ResultCount", 1),
                        new SqlParameter("@SearchType", 1),
                        new SqlParameter("@UpdateUser", 777),
                        new SqlParameter("@UpdateDate", DateTime.Now),
                    });

                unitOfWork.SearchTrackingRepository.Insert(st);
                unitOfWork.Commit();
                throw new InvalidOperationException();
            });
            Assert.Equals(initialCount, unitOfWork.SearchTrackingRepository.Get().Count());
        }
    }
}
