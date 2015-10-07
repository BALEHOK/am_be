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
        private readonly IFileService _fileService;
        private readonly IAssetService _assetService;

        public FilesController(IFileService fileService, IAssetService assetService)
        {
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            _fileService = fileService;
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;
        }

        [Route("")]
        public HttpResponseMessage Get(long assetTypeId, long attributeId, long assetId)
        {
            var attribute = _assetService.GetAssetAttribute(
                    assetTypeId, assetId, attributeId);
            var filePath = _fileService.GetRelativeFilepath(
                assetTypeId,
                attribute.Id,
                attribute.Datatype,
                attribute.Value.ToString());

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(HostingEnvironment.MapPath(filePath), FileMode.Open);
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
