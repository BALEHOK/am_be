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
            Mock<IEnvironmentSettings> envSettingsMock,
            Mock<IAttributeRepository> attributeRepositoryMock)
        {
            // Arrange
            envSettingsMock.Setup(x => x.GetDocsUploadDirectory(assetTypeId, attributeId))
                .Returns(DOCS_UPLOADS_DIR);

            attributeRepositoryMock
                .Setup(x => x.GetPublishedById(
                    It.IsAny<long>(),
                    It.IsAny<Expression<Func<DynEntityAttribConfig, object>>>()))
                .Returns(new DynEntityAttribConfig
                {
                    DataType = new DataType {Name = "file"}
                });

            var sut = new FileService(envSettingsMock.Object, attributeRepositoryMock.Object);
            // Act
            var result = sut.GetAttributeMediaType(assetTypeId, attributeId);
            // Assert
            Assert.Equal(Enumerators.MediaType.File, result);
        }

        [Theory, AutoDomainData]
        public void FileService_GetAttributeMediaType_ShouldReturnImageMediaType(
            long assetTypeId,
            long attributeId,
            Mock<IEnvironmentSettings> envSettingsMock,
            Mock<IAttributeRepository> attributeRepositoryMock)
        {
            // Arrange
            envSettingsMock.Setup(x => x.GetDocsUploadDirectory(assetTypeId, attributeId))
                .Returns(DOCS_UPLOADS_DIR);
            attributeRepositoryMock
                .Setup(x => x.GetPublishedById(
                    It.IsAny<long>(),
                    It.IsAny<Expression<Func<DynEntityAttribConfig, object>>>()))
                .Returns(new DynEntityAttribConfig
                {
                    DataType = new DataType {Name = "image"}
                });
            var sut = new FileService(envSettingsMock.Object, attributeRepositoryMock.Object);
            // Act
            var result = sut.GetAttributeMediaType(assetTypeId, attributeId);
            // Assert
            Assert.Equal(Enumerators.MediaType.Image, result);
        }

        [Theory, AutoDomainData]
        public void FileService_GetRelativeFilepathByIds_ReturnsFilepath(
            long assetTypeId,
            long attributeId,
            long assetId,
            Mock<IEnvironmentSettings> envSettingsMock,
            Mock<IAttributeRepository> attributeRepositoryMock)
        {
            // Arrange
            var anonymousFilename = "picture.jpg";
            var expected = string.Format("{0}/{1}", IMAGES_UPLOADS_DIR, anonymousFilename);
            envSettingsMock.Setup(x => x.GetImagesUploadDirectory(assetTypeId, attributeId))
                .Returns(IMAGES_UPLOADS_DIR);
            var sut = new FileService(envSettingsMock.Object, attributeRepositoryMock.Object);
            // Act
            var result = sut.GetRelativeFilepath(assetTypeId, attributeId, "image", anonymousFilename);
            // Assert
            Assert.Equal(expected, result);
        }
    }
}