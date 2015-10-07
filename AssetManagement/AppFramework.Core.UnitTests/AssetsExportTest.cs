
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.DAL;
using AppFramework.DataProxy;
using Moq;
using Ploeh.AutoFixture;
using System.Data;
using Xunit;

namespace AppFramework.Core.UnitTests
{
	public class AssetsExportTest
	{
		public AssetsExportTest()
		{

		}

		[Fact(Skip = "Refactoring is not completed")]
		public void AssetsExporter_ExportToDataTable_ReturnsDataTable()
		{
            //// Arrange
            //var fixture = new Fixture();
            //var atRepoMock = new Mock<IAssetTypeRepository>();
            //var accessManagerStub = new Mock<IAccessManager>();
            //var uowStub = new Mock<IUnitOfWork>();
            //var tableProviderStub = new Mock<ITableProvider>();
            //atRepoMock.Setup(r => r.GetByUid(It.IsAny<long>()))
            //    .Returns(new AssetType(accessManagerStub.Object, uowStub.Object, tableProviderStub.Object));
            //var uowMock = new Mock<IUnitOfWork>();
            //fixture.Register(() => atRepoMock.Object);
            //fixture.Register(() => uowMock.Object);
            //var sut = fixture.Create<AssetsExporter>();
            //// Act
            //var dataTable = sut.ExportToDataTable(0, null);
            //// Assert
            //Assert.NotNull(dataTable);
            //Assert.IsType<DataTable>(dataTable);
		}
	}
}
