using AssetManager.Infrastructure.Services;
using Common.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace AssetManager.WebApi.Controllers.Api
{
    // TODO: add cookie auth (this will be opened in a separate window)
    [AllowAnonymous]
    [RoutePrefix("api/files")]
    public class FilesController : ApiController
    {
        private readonly IFileService _fileService;
        private readonly IAssetService _assetService;
        private readonly ILog _logger;

        public FilesController(
            IFileService fileService, 
            IAssetService assetService,
            ILog logger)
        {
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            _fileService = fileService;
            if (assetService == null)
                throw new ArgumentNullException("assetService");
            _assetService = assetService;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        [Route("")]
        public HttpResponseMessage Get(long assetTypeId, long attributeId, long assetId)
        {
            // TODO: replace by actual user id
            var userId = 1;

            var attribute = _assetService.GetAssetAttribute(
                assetTypeId, assetId, attributeId, userId);

            var filePath = _fileService.GetFilepath(attribute);

            if (File.Exists(filePath))
            {
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
            else
            {
                _logger.WarnFormat("File not found: {0} (assetTypeId {1}, attributeId {2}, assetId {3})",
                    filePath, assetTypeId, attributeId, assetId);

                return Request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    "File not found");
            }
        }
    }
}
