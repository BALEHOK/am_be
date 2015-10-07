using AppFramework.DataProxy;
using AssetManager.WebApi.Controllers.Api;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Controllers
{
    public class ZipCodesControllerTest
    {
        private IQueryable<AppFramework.Entities.ZipCode> _anonymousCodes =
            new List<AppFramework.Entities.ZipCode>
            {
                new AppFramework.Entities.ZipCode { Code = "1000" },
                new AppFramework.Entities.ZipCode { Code = "1001" },
                new AppFramework.Entities.ZipCode { Code = "2000" },
                new AppFramework.Entities.ZipCode { Code = "2200" },
            }.AsQueryable();

        [Theory, AutoDomainData]
        public void ZipCodesController_ReturnsCollectionOfZipCodes(
            List<AppFramework.Entities.ZipCode> zipCodes,
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            ZipCodesController sut)
        {
            // Arrange
            unitOfWorkMock.Setup(x => x.ZipCodeRepository.AsQueryable())
                .Returns(zipCodes.AsQueryable());
            // Act
            var result = sut.Get();
            // Assert
            Assert.NotEmpty(result);
        }

        [Theory, AutoDomainData]
        public void ZipCodesController_GetZipCodesWithFilter_ReturnsMatchedZipCodes(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            ZipCodesController sut)
        {
            // Arrange
            var filter = "10";
            unitOfWorkMock.Setup(x => x.ZipCodeRepository.AsQueryable())
                .Returns(_anonymousCodes);
            // Act
            var result = sut.Get(filter, null, null);
            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.All(p => p.Code.ToLower().StartsWith(filter)));
        }

        [Theory, AutoDomainData]
        public void ZipCodesController_GetZipCodesWithRowStartAndRowNumber_ReturnsSubsetOfZipCodes(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            ZipCodesController sut)
        {
            // Arrange
            unitOfWorkMock.Setup(x => x.ZipCodeRepository.AsQueryable())
                .Returns(_anonymousCodes);
            // Act
            var result = sut.Get(null, 1, 1);
            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal("1001", result.Single().Code);
        }
    }
}
