using AppFramework.Core.AC.Authentication;
using AppFramework.DataProxy;
using AppFramework.DataProxy.Providers;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System.Data;

namespace AssetSite.UnitTests.Fixtures
{
    class AssetReportDataProviderFixture : Fixture
    {
        public AssetReportDataProviderFixture()
        {
            this.Customize(new AutoMoqCustomization());
            this.Register(() =>
            {
                var amStub = new Mock<IAccessManager>();
                return amStub.Object;
            });
            this.Register(() =>
            {
                var uowMock = new Mock<IUnitOfWork>();
                var dataProviderMock = new Mock<IDataProvider>();
                dataProviderMock.Setup(
                    p => p.ExecuteReader(It.IsAny<string>(), It.IsAny<IDataParameter[]>(), CommandType.Text))
                    .Returns(new Mock<IDataReader>().Object);
                uowMock.Setup(u => u.SqlProvider)
                    .Returns(dataProviderMock.Object);
                return uowMock.Object;
            });
            
            //this.Register(() =>
            //{
            ////var repositoryStub = new Mock<IAssetTypeRepository>();
            ////repositoryStub.Setup(r => r.GetById(It.IsAny<long>()))
            ////    .Returns(new AssetType(amStub.Object, uowMock.Object));
            //});
        }
    }
}
