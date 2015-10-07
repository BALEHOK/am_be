using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using AssetManager.Infrastructure.Models;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;
using Xunit.Extensions;
using AssetManager.WebApi.Controllers;
using AssetManager.WebApi.Controllers.Api;

namespace AssetSite.UnitTests.Controllers
{
    public class UploadsControllerTest
    {
        const string DOCS_UPLOADS_DIR = "~/App_Data/uploads";
        const string DOCS_UPLOADS_DIRPATH = @"C:\Site\App_Data\uploads";
        const string IMAGES_UPLOADS_DIR = "/uploads";
        const string IMAGES_UPLOADS_DIRPATH = @"C:\Site\uploads";

        [Theory, AutoDomainData]
        public void UploadsController_UploadAction_OnImageUpload_ShouldReturnImageUrl(
            long assetTypeId,            
            long attributeId,
            long? assetId,
            [Frozen]Mock<IFileService> fileServiceMock)
        {
            string fileName = "image.jpg";
            string uploadedFilename = @"\tempdir\" + fileName;
            var destinationPath = Path.Combine(IMAGES_UPLOADS_DIRPATH, fileName);

            fileServiceMock.Setup(x => x.GetImagesUploadDirectory(assetTypeId, attributeId))
                .Returns(IMAGES_UPLOADS_DIR);
            fileServiceMock.Setup(x => x.GetDestinationFilename(uploadedFilename, IMAGES_UPLOADS_DIR))
                .Returns(destinationPath);

            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            serverMock.Setup(x => x.MapPath(IMAGES_UPLOADS_DIR)).Returns(IMAGES_UPLOADS_DIRPATH);
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);
            
            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(x => x.FileName).Returns(uploadedFilename);
            fileMock.Setup(x => x.ContentLength).Returns(1024);

            //var sut = new UploadsController(fileServiceMock.Object);
            //var controllerContext = new ControllerContext(httpContextMock.Object, new RouteData(), sut);
            //sut.ControllerContext = controllerContext;

            //// act
            //var result = sut.PostFile(assetTypeId, attributeId, fileMock.Object, assetId);
            //var model = (result as JsonResult).Data as UploadResultModel;

            //// assert
            //fileMock.Verify(x => x.SaveAs(destinationPath));
            //Assert.IsType<JsonResult>(result);
            //Assert.NotNull(model);
            //Assert.Equal(fileName, model.Filename);
            //Assert.Equal(
            //    string.Format("{0}/{1}", IMAGES_UPLOADS_DIR, fileName),
            //    model.ImageUrl);
        }        
    }
}
