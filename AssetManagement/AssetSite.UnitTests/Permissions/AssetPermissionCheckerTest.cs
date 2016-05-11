using System.Collections.Generic;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AppFramework.UnitTests.Common.Fixtures;
using AssetManager.Infrastructure.Permissions;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppFramework.Web.UnitTests
{
    public class AssetPermissionCheckerTest
    {
        [Theory, AutoDomainData]
        public void AssetPermission_GetPermissionOnChildEmployee_ReturnsPermissionOnUsers(
            long userId,
            IUnitOfWork unitOfWork,
            [Frozen]Mock<IAssetTypeRepository> assetTypeRepoMock,
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            [Frozen]Mock<IAssetsService> assetsServiceMock,
            AssetPermissionChecker sut)
        {
            // Arrange
            var expected = Permission.RDDD;
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new UserAssetCustomization(unitOfWork))
                .Customize(new AssetTypeCustomization(unitOfWork));

            var asset = fixture.Create<Asset>();
            var currentUser = fixture.Create<Asset>();
            currentUser["PermissionOnUsers"].Value = expected.GetCode().ToString();
            var userType = fixture.Create<AssetType>();

            assetTypeRepoMock
                .Setup(x => x.IsPredefinedAssetType(
                    asset.Configuration, PredefinedEntity.User))
                .Returns(true);

            assetTypeRepoMock
                .Setup(x => x.GetPredefinedAssetType(PredefinedEntity.User))
                .Returns(userType);

            assetsServiceMock
                .Setup(x => x.GetAssetById(userId, userType))
                .Returns(currentUser);

            unitOfWorkMock
                .Setup(x => x.GetUsersTree(userId))
                .Returns(new List<long> { asset.ID });

            // Act 
            var result = sut.GetPermission(asset, userId);
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory(Skip = "To be implemented"), AutoDomainData]
        public void AssetPermission_GetPermissionOnAssetWhichBelongsToMe_ReturnsPermissionOnAsset(
            AssetPermissionChecker sut)
        { }

        [Theory(Skip = "To be implemented"), AutoDomainData]
        public void AssetPermission_GetPermissionOnAssetWhichBelongsToMyEmployee_ReturnsPermissionOnUsers(
            AssetPermissionChecker sut)
        { }

        [Theory(Skip = "To be implemented"), AutoDomainData]
        public void AssetPermission_GetPermissionOnOtherAsset_ReturnsPermissionOnAsset(
            AssetPermissionChecker sut)
        { }
    }
}
