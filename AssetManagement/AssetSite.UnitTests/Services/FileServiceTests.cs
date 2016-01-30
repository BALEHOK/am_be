using System;
using System.Linq.Expressions;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Entities;
using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Services;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Xunit;
using Xunit.Extensions;
using Ploeh.AutoFixture.Xunit;

namespace AssetSite.UnitTests.Services
{
    public class FileServiceTests
    {
        const string DOCS_UPLOADS_DIR = "~/App_Data/uploads";
        const string IMAGES_UPLOADS_DIR = "~/uploads";

        [Theory, AutoDomainData]
        public void FileService_GetAttributeMediaType_ShouldReturnFileMediaType(
            long assetTypeId,
            long attributeId,
            [Frozen]Mock<IEnvironmentSettings> envSettingsMock,
            [Frozen]Mock<IAttributeRepository> attributeRepositoryMock,
            FileService sut)
        {
            // Arrange
            envSettingsMock.Setup(x => x.GetDocsUploadBaseDir())
                .Returns(DOCS_UPLOADS_DIR);

            attributeRepositoryMock
                .Setup(x => x.GetPublishedById(
                    It.IsAny<long>(),
                    It.IsAny<Expression<Func<DynEntityAttribConfig, object>>>()))
                .Returns(new DynEntityAttribConfig
                {
                    DataType = new DataType {Name = "file"}
                });

            // Act
            var result = sut.GetAttributeMediaType(assetTypeId, attributeId);
            // Assert
            Assert.Equal(Enumerators.MediaType.File, result);
        }

        [Theory, AutoDomainData]
        public void FileService_GetAttributeMediaType_ShouldReturnImageMediaType(
            long assetTypeId,
            long attributeId,
            [Frozen]Mock<IEnvironmentSettings> envSettingsMock,
            [Frozen]Mock<IAttributeRepository> attributeRepositoryMock,
            FileService sut)
        {
            // Arrange
            envSettingsMock.Setup(x => x.GetDocsUploadBaseDir())
                .Returns(DOCS_UPLOADS_DIR);
            attributeRepositoryMock
                .Setup(x => x.GetPublishedById(
                    It.IsAny<long>(),
                    It.IsAny<Expression<Func<DynEntityAttribConfig, object>>>()))
                .Returns(new DynEntityAttribConfig
                {
                    DataType = new DataType {Name = "image"}
                });

            // Act
            var result = sut.GetAttributeMediaType(assetTypeId, attributeId);
            // Assert
            Assert.Equal(Enumerators.MediaType.Image, result);
        }
    }
}