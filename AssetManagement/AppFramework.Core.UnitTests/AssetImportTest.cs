using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.IE.Adapters;
using AppFramework.Core.DAL;
using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.UnitTests.Fixtures;
using AppFramework.Core.UnitTests.Fixtures.AssetTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.UnitTests.Common.Fixtures;
using Common.Logging;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using Xunit.Extensions;
using Resources = AppFramework.Core.UnitTests.Properties.Resources;

namespace AppFramework.Core.UnitTests
{
    public class AssetImportTest
    {
        [Theory(Skip="To be fixed"), AutoDomainData]
        public void AssetImportTest_Import_ReturnsAssetsList(
            long assetTypeId,
            long userId,
            string filepath,
            List<string> sheets, 
            [Frozen] IUnitOfWork unitOfWork,
            [Frozen] ILog logger,
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository,
            IBarcodeProvider barcodeProvider)
        {
            // Fixture setup
            var linkedEntityFinder = new LinkedEntityFinderFixture();
            var xmlToAssetsAdapter = new XMLToAssetsAdapter(assetsService, assetTypeRepository, linkedEntityFinder, barcodeProvider, logger);
            var anonymousXml = XDocument.Parse(Resources.ImportXml);

            var repo = new MockRepository(MockBehavior.Default);
            var dynColumnAdapter = repo.Create<IDynColumnAdapter>();
            dynColumnAdapter.Setup(a => a.ConvertDynEntityAttribConfigToDynColumn(It.IsAny<DynEntityAttribConfig>()))
                .Returns<DynEntityAttribConfig>(a => new DynColumn { Name = a.DBTableFieldname });

            var anonymousAssetType = new WerkloosheidAssetTypeFixture(
                unitOfWork);
            var bindings = new WerkloosheidBindingsFixture(anonymousAssetType);

            var repositoryMock = repo.Create<IAssetTypeRepository>();
            repositoryMock.Setup(r => r.GetById(It.IsAny<long>()))
                .Returns(anonymousAssetType);
            var excelToXmlAdapterMock = repo.Create<IExcelToXmlConverter>();
            excelToXmlAdapterMock.Setup(a => a.ConvertToXml(It.IsAny<string>(),
                It.IsAny<BindingInfo>(), It.IsAny<AssetType>(), It.IsAny<List<string>>()))
                .Returns(anonymousXml);
            
            var sut = new AssetsImporter(
                repositoryMock.Object, 
                excelToXmlAdapterMock.Object, 
                xmlToAssetsAdapter,
                logger);
            
            // Exercise system
            var result = sut.Import(assetTypeId, userId, filepath, sheets, bindings).ToList();
            // Verify outcome
            Assert.NotEmpty(result);
            Assert.Equal("EL ALLAOUI, ALI", result.First().Name);
            // Teardown
        }
    }
}
