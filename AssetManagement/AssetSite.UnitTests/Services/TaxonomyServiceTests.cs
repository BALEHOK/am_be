using System.Collections.Generic;
using System.Linq;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.UnitTests.Common.Fixtures;
using AssetManager.Infrastructure.Services;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Services
{
    public class TaxonomyServiceTests
    {
        [Theory, AutoDomainData]
        public void TaxonomyService_ReturnsTaxonomyModel(
            [Frozen]Mock<AppFramework.Core.Classes.IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            TaxonomyService sut,
            long assetTypeId)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetTypeCustomization(unitOfWorkMock.Object));

            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId, It.IsAny<bool>()))
                .Returns(assetType);

            unitOfWorkMock
                .Setup(uow => uow
                    .TaxonomyItemRepository
                    .GetTaxonomyItemsByAssetTypeId(assetTypeId))
                .Returns(new List<TaxonomyItem> {
                    new TaxonomyItem
                    {
                        Name = "second",
                        ParentItem = new TaxonomyItem
                        {
                            Name = "first"
                        },
                        Taxonomy = new Taxonomy
                        {
                            IsCategory = true
                        }
                    }});
            //Act
            var result = sut.GetTaxonomyByAssetTypeId(assetTypeId);
            
            //Assert
            Assert.Equal(1, result.Count());

            var item = result.First();
            Assert.Equal("first", item.Name);
            Assert.Equal("second", item.Child.Name);
            Assert.Equal(assetType.NameInvariant, item.Child.AssetType.DisplayName);
        }
    }
}
