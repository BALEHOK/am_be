using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/files")]
    public class FilesController : ApiController
    {
        private readonly IAssetService _assetService;
        private readonly IEnvironmentSettings _envSettings;

        public FilesController(IEnvironmentSettings envSettings, IAssetService assetService)
        {
            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _envSettings = envSettings;
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;
        }

        [Route("")]
        public HttpResponseMessage Get(long assetTypeId, long attributeId, long assetId)
        {
            var attribute = _assetService.GetAssetAttribute(
                    assetTypeId, assetId, attributeId);
           
            var relPath = _envSettings.GetAssetMediaRelativePath(
                   assetTypeId, attribute.Id);
            var baseDir = attribute.Datatype == "image"
                ? _envSettings.GetImagesUploadBaseDir()
                : _envSettings.GetDocsUploadBaseDir();
            var filePath = HostingEnvironment.MapPath(
                Path.Combine(baseDir, relPath, attribute.Value.ToString()));

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(filePath, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(filePath)
                };
            return result;
        }
    }
}
