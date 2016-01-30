using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using AssetManager.WebApi.Controllers.Api;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Controllers
{
    public class UploadsControllerTest
    {
        const string MEDIA_BASEDIR = @"D:\uploads";
        const string MEDIA_HTTPROOT = "http://assetmanager/media/";
        const string FILENAME = "image.jpg";

        [Theory, AutoDomainData]
        public async Task UploadsController_UploadAction_OnImageUpload_ShouldReturnImageUrl(
            long assetTypeId,            
            long attributeId,
            long? assetId,
            [Frozen]Mock<IEnvironmentSettings> envSettingsMock,
            [Frozen]Mock<IFileService> fileServiceMock,
            UploadsController sut)
        {
            // arrange
            var relativePath = string.Format("/assets/{0}/{1}", assetTypeId, attributeId);
            var fullPath = string.Format("{0}/{1}", MEDIA_BASEDIR, relativePath);
            var destinationFilename = Path.Combine(fullPath, FILENAME);

            sut.Request = new HttpRequestMessage();
            sut.Configuration = new System.Web.Http.HttpConfiguration();
            sut.Request.Content = new MultipartFormDataContent();

            envSettingsMock.Setup(x => x.GetAssetMediaRelativePath(assetTypeId, attributeId))
                .Returns(relativePath);
            envSettingsMock.Setup(x => x.GetImagesUploadBaseDir())
                .Returns(MEDIA_BASEDIR);
            envSettingsMock.Setup(x => x.GetAssetMediaHttpRoot())
                .Returns(MEDIA_HTTPROOT);

            fileServiceMock.Setup(x => x.UploadFile(It.IsAny<IEnumerable<MultipartFileData>>(), fullPath))
                .Returns(new FileInfo(destinationFilename));

            // act
            var result = await sut.PostFile(assetTypeId, attributeId, assetId);
            var model = result.Content.ReadAsAsync<UploadResultModel>().Result;

            // assert
            Assert.Equal(FILENAME, model.Filename);
            Assert.Equal(
                string.Format("{0}/{1}/{2}", MEDIA_HTTPROOT, relativePath, FILENAME), 
                model.ImageUrl);
        }        
    }
}
