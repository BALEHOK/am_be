using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.DAL;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using Moq;
using Xunit;

namespace AssetSite.UnitTests
{
    public class AssetsListReportDataProviderTest
    {
        [Fact(Skip="")]
        public void AssetsListReportDataProvider_WhenNoAssetTypeByGivenId_Throws()
        {
            // Arrange
            var anonymousId = 123;
            var repositoryStub = new Mock<IAssetTypeRepository>();
            var serviceStub = new Mock<IAssetsService>();
            // Act
            // Assert
            Assert.Throws<AssetTypeNotFoundException>(() => new AssetSite.Reports.DataProviders.AssetsListReportDataProvider(anonymousId, repositoryStub.Object, serviceStub.Object));
        }

        [Fact(Skip = "Cache Dependency")]
        public void AssetsListReportDataProvider_GetData_ReturnsDataTable()
        {
            //// Arrange
            //var accessManagerStub = new Mock<IAccessManager>();
            //var uowStub = new Mock<IUnitOfWork>();
            //var tableProviderStub = new Mock<ITableProvider>();
            //var anonymousAssetType = new AssetType(accessManagerStub.Object, uowStub.Object, tableProviderStub.Object);
            //var anonymousId = 123;
            //var repositoryStub = new Mock<IAssetTypeRepository>();
            //repositoryStub.Setup(r => r.GetById(It.Is<long>(v => v == 123)))
            //    .Returns(anonymousAssetType);
            //var sut = new AssetSite.Reports.DataProviders.AssetsListReportDataProvider(anonymousId, repositoryStub.Object);
            //// Act
            //var result = sut.GetData();
            //// Assert
            //Assert.NotNull(result);
        }
    }
}
