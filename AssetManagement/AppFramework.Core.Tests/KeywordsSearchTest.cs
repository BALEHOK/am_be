using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppFramework.Core.Tests
{
    [TestClass]
    public class KeywordsSearchTest
    {
        [TestMethod]
        public void SearchByKeywordsDALTest()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var result = unitOfWork.SearchByKeywords(
                123423,
                1,
                "ha",
                string.Empty,
                string.Empty,
                true,
                Entities.Enumerations.SearchOrder.Date,
                1,
                20);
            
            Assert.IsTrue(result.Count() > 0);
        }
    }
}
