using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE.Adapters;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.UnitTests.Fixtures;
using AppFramework.Core.UnitTests.Fixtures.AssetTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.UnitTests.Common.Fixtures;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using Xunit.Extensions;

namespace AppFramework.Core.UnitTests
{
    public class ExcelToXmlConverterTests
    {
        private readonly XNamespace _namespace = "http://tempuri.org/AssetManagementAssets.xsd";

        [Theory(Skip = "To be refactored"), AutoDomainData]
        public void ExcelToXmlConverterTests_ConvertToXml_ReturnsXDocument(
            IUnitOfWork unitOfWork,
            List<string> sheets)
        {
            // Arrange
            var repo = new MockRepository(MockBehavior.Default);
            var dynColumnAdapter = repo.Create<IDynColumnAdapter>();
            dynColumnAdapter.Setup(a => a.ConvertDynEntityAttribConfigToDynColumn(It.IsAny<DynEntityAttribConfig>()))
                .Returns<DynEntityAttribConfig>(a => new DynColumn { Name = a.DBTableFieldname });

            var anonymousAssetType = new WerkloosheidAssetTypeFixture(
                unitOfWork);
            var bindings = new WerkloosheidBindingsFixture(anonymousAssetType);

            var excelFilePath =
                @"D:\projects\sobenbub\AssetManagement\AppFramework.Core.UnitTests\Resources\Import_Werklosheid.xlsx";
            var sut = new ExcelToXmlConverter();
            // Act
            var result = sut.ConvertToXml(excelFilePath, bindings, anonymousAssetType, new List<string>(new[]{"Sheet1"}));
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Descendants());
        }
    }
}
