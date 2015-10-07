using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DataTypes;
using AppFramework.Core.DTO;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Services;
using AssetManager.Infrastructure.Models;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using AppFramework.UnitTests.Common.Fixtures;

namespace AssetSite.UnitTests.Services
{
    public class AssetServiceTests
    {
        [Theory, AutoDomainData]
        public void AssetService_GetAsset_ReturnsModel(
            long assetTypeId,
            long assetId,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IAuthenticationService> authServiceMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var asset = fixture.Create<AppFramework.Core.Classes.Asset>();
            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetAssetById(assetId, assetType))
                .Returns(asset);

            authServiceMock
                .Setup(s => s.GetPermission(asset))
                .Returns(Permission.RWRW);

            //Act
            var result = sut.GetAsset(assetTypeId, assetId);
            //Assert
            Assert.NotNull(result);
            Assert.True(result.Editable);
            Assert.False(result.Deletable);
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetByRevision_ReturnsModel(
            long assetTypeId,
            long assetId,
            int revision,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IAuthenticationService> authServiceMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var asset = fixture.Create<AppFramework.Core.Classes.Asset>();
            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetAssetByIdAndRevison(assetId, assetType, revision))
                .Returns(asset);

            authServiceMock
                .Setup(s => s.GetPermission(asset))
                .Returns(Permission.RWRW);

            //Act
            var result = sut.GetAsset(assetTypeId, assetId, revision);
            //Assert
            Assert.NotNull(result);
            Assert.True(result.Editable);
            Assert.False(result.Deletable);
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetByUid_ReturnsModel(
            long assetTypeId,
            long assetId,
            long uid,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IAuthenticationService> authServiceMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var asset = fixture.Create<AppFramework.Core.Classes.Asset>();
            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetAssetByUid(uid, assetType))
                .Returns(asset);

            authServiceMock
                .Setup(s => s.GetPermission(asset))
                .Returns(Permission.RWRW);

            //Act
            var result = sut.GetAsset(assetTypeId, assetId, null, uid);
            //Assert
            Assert.NotNull(result);
            Assert.True(result.Editable);
            Assert.False(result.Deletable);
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetByUidAndRevision_Throws(
            long assetTypeId,
            long assetId,
            int revision,
            long uid,
            AssetService sut)
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => 
                sut.GetAsset(assetTypeId, assetId, revision, uid));
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetRelatedEntities_ReturnsListOfAttributesWithAssets(
            long assetTypeId,
            long assetId,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var asset = fixture.Create<AppFramework.Core.Classes.Asset>();
            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetRelatedAssetByAttribute(It.IsAny<AssetAttribute>()))
                .Returns(asset);

            assetsCoreServiceMock
                .Setup(s => s.GetRelatedAssetsByAttribute(It.IsAny<AssetAttribute>()))
                .Returns(new List<PlainAssetDTO> 
                { 
                    new PlainAssetDTO{Id = 1, Name ="asset 1" },
                    new PlainAssetDTO{Id = 2, Name ="asset 2" },
                });

            assetsCoreServiceMock
                .Setup(s => s.GetAssetById(assetId, assetType))
                .Returns(asset);

            //Act
            var result = sut.GetAssetRelatedEntities(assetTypeId, assetId);
            //Assert
            Assert.NotEmpty(result);
            // RelatedAsset
            Assert.True(result.Single(r => r.Datatype == "asset").Assets.Count() == 1);
            // RelatedAssets
            Assert.True(result.Single(r => r.Datatype == "assets").Assets.Count() == 2);
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetRelatedEntities_ReturnsListOfAttributesWithDynLists(
            long assetTypeId,
            long assetId,
            long dynListUid,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IDynamicListsService> dynListServiceMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));
            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId))
                .Returns(assetType);

            dynListServiceMock
                .Setup(s => s.GetByUid(dynListUid))
                .Returns(new DynamicList
                {
                    Items = new List<DynamicListItem>
                    {
                        new DynamicListItem()
                    }
                });

            assetsCoreServiceMock
                .Setup(s => s.GetAssetById(assetId, assetType))
                .Returns(new AppFramework.Core.Classes.Asset
                {
                    Attributes = new List<AssetAttribute>
                    {
                        new AssetAttribute
                        {
                            Configuration = new AssetTypeAttribute(
                                new DynEntityAttribConfig(), unitOfWork)
                            {
                                DataType = new CustomDataType
                                {
                                    Code = Enumerators.DataType.DynList
                                },
                                DynamicListUid = dynListUid
                            }                            
                        },
                        new AssetAttribute
                        {
                            Configuration = new AssetTypeAttribute(
                                new DynEntityAttribConfig(), unitOfWork)
                            {
                                DataType = new CustomDataType
                                {
                                    Code = Enumerators.DataType.DynLists
                                },
                                DynamicListUid = dynListUid
                            }
                        }
                    }
                });
            //Act
            var result = sut.GetAssetRelatedEntities(assetTypeId, assetId);
            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            // DynList
            Assert.NotNull(result.First().List);
            Assert.NotEmpty(result.First().List.Items);
            Assert.Equal(1, result.First().List.Items.Count());
            // DynList
            //Assert.NotEmpty(result.Skip(1).First().Assets);
            //Assert.Equal(2, result.Skip(1).First().Assets.Count());
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetHistory_ReturnsHistoryModel(
            long assetTypeId,
            long assetId,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepoMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            // Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));
            var userAssetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            assetTypeRepoMock
                .Setup(r => r.GetPredefinedAssetType(PredefinedEntity.User))
                .Returns(userAssetType);

            var historyAssets = fixture.Create<List<AppFramework.Core.Classes.Asset>>();
            historyAssets[0][AttributeNames.Revision].Value = "1";
            historyAssets[1][AttributeNames.Revision].Value = "2";
            historyAssets[2][AttributeNames.Revision].Value = "3";

            historyAssets[0][AttributeNames.Name].Value = "asset 1";
            historyAssets[1][AttributeNames.Name].Value = "asset 1-1";

            assetsCoreServiceMock
                .Setup(s => s.GetHistoryAssets(assetTypeId, assetId)) 
                .Returns(historyAssets);

            // Act 
            var result = sut.GetAssetHistory(assetTypeId, assetId);
            // Assert
            Assert.NotEmpty(result.Revisions);
            Assert.NotEqual(
                result.Revisions[0].RevisionNumber, 
                result.Revisions[1].RevisionNumber);
            Assert.Empty(result.Revisions[0].ChangedValues);
            Assert.Equal(1, result.Revisions[1].ChangedValues.Count);
        }

        [Theory, AutoDomainData]
        public void AssetService_SaveAsset_ConvertsModelToAsset(
            AssetModel model,
            long userId,
            IUnitOfWork unitOfWork,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IAssetsService> assetsServiceMock,
            AssetService sut)
        {
            // Arrange
            model.Screens.First().IsDefault = true;
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var assetType = fixture.Create<AssetType>();
            var asset = fixture.Create<AppFramework.Core.Classes.Asset>();
            asset.Configuration = assetType;

            assetTypeRepositoryMock
                .Setup(r => r.GetById(model.AssetTypeId))
                .Returns(assetType);

            assetsServiceMock
               .Setup(s => s.GetAssetById(model.Id, assetType))
               .Returns(asset);

            // Act 
            sut.SaveAsset(model, userId);
            // Assert
            assetsServiceMock.Verify(s => s.InsertAsset(asset));
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetWithRoleAttribute_ReturnsRoleId(
            long assetTypeId,
            long assetId,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepoMock,
            [Frozen]Mock<IAuthenticationService> authServiceMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            // Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var assetType = fixture.Create<AssetType>();
            var asset = fixture.Create<AppFramework.Core.Classes.Asset>();

            assetTypeRepoMock
                .Setup(x => x.GetById(assetTypeId))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetAssetById(assetId, assetType))
                .Returns(asset);

            authServiceMock
                .Setup(x => x.GetPermission(asset))
                .Returns(Permission.RDDD);

            // Act 
            var result = sut.GetAsset(assetTypeId, assetId);
            // Assert

        }

        [Theory, AutoDomainData]
        public void AssetService_SaveAssetWithNewDocument_CreatesRelatedDocumentAsset(
            AssetService sut)
        {
            // Arrange
            // Act 
            //var result = sut.SaveAsset();
            // Assert
            throw new NotImplementedException("");
        }
    }
}
