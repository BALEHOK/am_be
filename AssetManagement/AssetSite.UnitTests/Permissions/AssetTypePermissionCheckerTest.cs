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
using AppFramework.Entities;
using System.Collections.Generic;
using System.Linq;

namespace AssetSite.UnitTests.Permissions
{
    public class AssetTypePermissionCheckerTest
    {
        [Theory, AutoDomainData]
        public void AssetTypePermission_FilterReadPermitted_ReturnsReadPermittedAssetTypes(
            long userId,
            DynEntityConfig readPermittedAssetType,
            DynEntityConfig forbiddenAssetType,
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            AssetTypePermissionChecker sut)
        {
            // Arrange
            var readPermission = Permission.RDDD;

            unitOfWorkMock
                .Setup(x => x.GetPermittedAssetTypes(userId, (byte)readPermission))
                .Returns(new[] { readPermittedAssetType.DynEntityConfigId });

            // Act 
            var result = sut.FilterReadPermitted(new[] { readPermittedAssetType, forbiddenAssetType }, userId).ToList();
            // Assert
            Assert.Equal(1, result.Count);
            Assert.Equal(readPermittedAssetType.DynEntityConfigId, result[0].DynEntityConfigId);
        }

        [Theory, AutoDomainData]
        public void AssetTypePermission_FilterWritePermitted_ReturnsWritePermittedAssetTypes(
            long userId,
            DynEntityConfig writePermittedAssetType,
            DynEntityConfig forbiddenAssetType,
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            AssetTypePermissionChecker sut)
        {
            // Arrange
            var writePermission = Permission.DWDD;

            unitOfWorkMock
                .Setup(x => x.GetPermittedAssetTypes(userId, (byte)writePermission))
                .Returns(new[] { writePermittedAssetType.DynEntityConfigId });

            // Act 
            var result = sut.FilterWritePermitted(new[] { writePermittedAssetType, forbiddenAssetType }, userId).ToList();
            // Assert
            Assert.Equal(1, result.Count);
            Assert.Equal(writePermittedAssetType.DynEntityConfigId, result[0].DynEntityConfigId);
        }
    }
}
