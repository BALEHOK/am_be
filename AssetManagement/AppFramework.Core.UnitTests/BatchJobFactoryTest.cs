using System;
using System.Collections.Generic;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Batch;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.UnitTests.Fixtures;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppFramework.Core.UnitTests
{
    public class BatchJobFactoryTest
    {
        [Theory, AutoDomainData]
        public void CreateSyncAssetsJobShouldReturnJobInstance(
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            IBatchJobManager manager,
            IBatchActionFactory batchActionFactory,
            BatchJobFactory sut)
        {
            // Arrange
            var anonymousAssettype = new AssetType(new DynEntityConfig {Name = "test"},
                unitOfWork);
            var assetTypeRepository = new Mock<IAssetTypeRepository>();
            assetTypeRepository.Setup(r => r.GetById(It.IsAny<long>(), It.IsAny<bool>()))
                .Returns(anonymousAssettype);
            // Act
            var actualJob = sut.CreateSyncAssetsJob(new SyncAssetsParameters(Guid.Empty, 0, 0, "", "", ""));
            // Assert
            Assert.NotNull(actualJob);
        }

        [Theory, AutoDomainData]
        public void CreateSyncAssetsJobTitleIsCorrect(
            string assetsIdentifier,
            string assetTypeName,
            IUnitOfWork unitOfWork,
            [Frozen] Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            BatchJobFactory sut)
        {
            // Arrange
            assetTypeRepositoryMock.Setup(m => m.GetById(It.IsAny<long>(), It.IsAny<bool>()))
                .Returns(new AssetType(new DynEntityConfig {Name = assetTypeName}, unitOfWork));
            // Act
            var actualJob = sut.CreateSyncAssetsJob(
                new SyncAssetsParameters(Guid.Empty, 0, 0, assetsIdentifier, "", ""));
            // Assert
            Assert.Equal(string.Format("Synchronizing {0} on key {1}", assetTypeName, assetsIdentifier),
                actualJob.Title);
        }

        [Theory, AutoDomainData]
        public void CreateImportAssetsJobReturnsJobInstance(
            long currentUserId,
            string filepath,
            long assetTypeId,
            BindingInfo bindings,
            List<string> sheets,
            IUnitOfWork unitOfWork,
            [Frozen] Mock<IAssetTypeRepository> assetTypeRepositoryMock,
            BatchJobFactory sut)
        {
            // Arrange
            assetTypeRepositoryMock.Setup(m => m.GetById(It.IsAny<long>(), It.IsAny<bool>()))
                .Returns(new AssetType(new DynEntityConfig {Name = "Test"}, unitOfWork));
            // Act
            var result = sut.CreateImportAssetsJob(currentUserId, filepath, assetTypeId, bindings, sheets, true);
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test import", result.Title);
        }
    }
}