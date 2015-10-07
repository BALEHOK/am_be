using AppFramework.Core.Classes;
using AppFramework.Core.DAL;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.DataProxy.Providers;
using AppFramework.Entities;
using AssetSite.Reports.DataProviders;
using Moq;
using System.Data;
using Xunit;

namespace AssetSite.UnitTests
{
    public class AssetReportDataProviderTest
    {
        [Fact(Skip="")]
        public void AssetReportDataProvider_WhenNoAssetTypeByGivenId_Throws()
        {
            // Arrange
            var anonymousAssetUid = 123;
            var anonymousAssetTypeId = 456;
            var repositoryStub = new Mock<IAssetTypeRepository>();
            var serviceStub = new Mock<IAssetsService>();
            // Act
            // Assert
            Assert.Throws<AssetTypeNotFoundException>(
                () => new AssetReportDataProvider(anonymousAssetUid, anonymousAssetTypeId, repositoryStub.Object, serviceStub.Object));
        }

        [Fact]
        public void AssetReportDataProvider_WhenNoAssetByGivenUid_Throws()
        {
            // Arrange
            var anonymousAssetUid = 123;
            var anonymousAssetTypeId = 456;
            var uowMock = new Mock<IUnitOfWork>();
            var dataProviderMock = new Mock<IDataProvider>();
            var serviceStub = new Mock<IAssetsService>();
            dataProviderMock.Setup(
                p => p.ExecuteReader(It.IsAny<string>(), It.IsAny<IDataParameter[]>(), CommandType.Text))
                .Returns(new Mock<IDataReader>().Object);
            uowMock.Setup(u => u.SqlProvider)
                .Returns(dataProviderMock.Object);
            var repositoryStub = new Mock<IAssetTypeRepository>();
            repositoryStub.Setup(r => r.GetById(It.IsAny<long>()))
                .Returns(new AssetType(
                    new DynEntityConfig(), uowMock.Object));
            // Act
            // Assert
            Assert.Throws<AssetNotFoundException>(
                () => new AssetReportDataProvider(anonymousAssetUid, anonymousAssetTypeId, repositoryStub.Object, serviceStub.Object));
        }

        [Fact(Skip = "Refactor to Fixture")]
        public void AssetReportDataProvider_GetData_ReturnsObject()
        {
            //// Arrange
            //var anonymousAssetUid = 123;
            //var anonymousAssetTypeId = 456;
            //var amStub = new Mock<IAccessManager>();
            //var uowMock = new Mock<IUnitOfWork>();
            //var tableProviderStub = new Mock<ITableProvider>();
            //var dataProviderMock = new Mock<IDataProvider>();
            //dataProviderMock.Setup(
            //    p => p.ExecuteReader(It.IsAny<string>(), It.IsAny<IDataParameter[]>(), CommandType.Text))
            //    .Returns(new Mock<IDataReader>().Object);
            //uowMock.Setup(u => u.SqlProvider)
            //    .Returns(dataProviderMock.Object);
            //var repositoryStub = new Mock<IAssetTypeRepository>();
            //repositoryStub.Setup(r => r.GetById(It.IsAny<long>()))
            //    .Returns(new AssetType(amStub.Object, uowMock.Object, tableProviderStub.Object));
            //var sut = new AssetReportDataProvider(anonymousAssetUid, anonymousAssetTypeId, repositoryStub.Object);
            //// Act
            //var result = sut.GetData();
            //// Assert
            //Assert.NotNull(result);
        }
    }
}
