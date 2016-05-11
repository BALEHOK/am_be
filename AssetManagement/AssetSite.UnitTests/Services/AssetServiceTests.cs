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
using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Permissions;

namespace AssetSite.UnitTests.Services
{
    public class AssetServiceTests
    {
        [Theory, AutoDomainData]
        public void ModelFactory_GetAssetModel_ReturnsModel(
            IUnitOfWork unitOfWork,
            ModelFactory sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));
            var asset = fixture.Create<AssetWrapperForScreenView>();
            //Act
            var result = sut.GetAssetModel(asset, Permission.RWRW);
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
            [Frozen]Mock<IAssetPermissionChecker> permissionCheckerMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var asset = fixture.Create<Asset>();
            var assetType = fixture.Create<AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId, It.IsAny<bool>()))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetAssetByIdAndRevison(assetId, assetType, revision))
                .Returns(asset);

            permissionCheckerMock
                .Setup(s => s.GetPermission(It.IsAny<Asset>(), It.IsAny<long>()))
                .Returns(Permission.RWRW);

            //Act
            var result = sut.GetAsset(assetTypeId, assetId, revision);
            //Assert
            Assert.NotNull(result);
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetByUid_ReturnsModel(
            long assetTypeId,
            long assetId,
            long userId,
            long uid,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IAssetPermissionChecker> permissionCheckerMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            //Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var asset = fixture.Create<Asset>();
            var assetType = fixture.Create<AssetType>();

            assetTypeRepositoryMock
                .Setup(r => r.GetById(assetTypeId, It.IsAny<bool>()))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetAssetByUid(uid, assetType))
                .Returns(asset);

            permissionCheckerMock
                .Setup(s => s.GetPermission(It.IsAny<Asset>(), It.IsAny<long>()))
                .Returns(Permission.RWRW);

            //Act
            var result = sut.GetAsset(assetTypeId, assetId, userId, null, uid);
            //Assert
            Assert.NotNull(result);
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetByUidAndRevision_Throws(
            long assetTypeId,
            long assetId,
            long userId,
            int revision,
            long uid,
            AssetService sut)
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => 
                sut.GetAsset(assetTypeId, assetId, userId, revision, uid));
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
                .Setup(r => r.GetById(assetTypeId, It.IsAny<bool>()))
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
                .Setup(r => r.GetById(assetTypeId, It.IsAny<bool>()))
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
            var userAssetType = fixture.Create<AssetType>();

            assetTypeRepoMock
                .Setup(r => r.GetPredefinedAssetType(PredefinedEntity.User))
                .Returns(userAssetType);

            var historyAssets = fixture.Create<List<Asset>>();
            historyAssets[0].Revision = 1;
            historyAssets[1].Revision = 2;
            historyAssets[2].Revision = 3;

            assetsCoreServiceMock
                .Setup(s => s.GetHistoryAssets(assetTypeId, assetId)) 
                .Returns(historyAssets);

            // Act 
            var result = sut.GetAssetHistory(assetTypeId, assetId);
            // Assert
            Assert.NotEmpty(result.Revisions);
            Assert.Equal(3, result.Revisions.Count);
        }

        [Theory, AutoDomainData]
        public void AssetService_GetAssetWithRoleAttribute_ReturnsRoleId(
            long assetTypeId,
            long assetId,
            long userId,
            [Frozen]Mock<IAssetsService> assetsCoreServiceMock,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepoMock,
            [Frozen]Mock<IAssetPermissionChecker> permissionCheckerMock,
            IUnitOfWork unitOfWork,
            AssetService sut)
        {
            // Arrange
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var assetType = fixture.Create<AssetType>();
            var asset = fixture.Create<Asset>();

            assetTypeRepoMock
                .Setup(x => x.GetById(assetTypeId, It.IsAny<bool>()))
                .Returns(assetType);

            assetsCoreServiceMock
                .Setup(s => s.GetAssetById(assetId, assetType))
                .Returns(asset);

            permissionCheckerMock
                .Setup(s => s.GetPermission(It.IsAny<Asset>(), It.IsAny<long>()))
                .Returns(Permission.RDDD);

            // Act 
            var result = sut.GetAsset(assetTypeId, assetId, userId);
            // Assert

        }

        [Theory(Skip = "TBD"), AutoDomainData]
        public void AssetService_SaveAssetWithNewDocument_CreatesRelatedDocumentAsset(
            AssetService sut)
        {
            // Arrange
            // Act 
            // Assert
        }
    }
}
