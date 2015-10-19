using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using AppFramework.ConstantsEnumerators;
using Common.Logging;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/uploads")]
    public class UploadsController : ApiController
    {
        private readonly IFileService _fileService;
        private readonly IValidationService _validationService;
        private readonly ILog _logger;

        public UploadsController(
            IFileService fileService,
            IValidationService validationService,
            ILog logger)
        {
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            _fileService = fileService;
            if (validationService == null)
                throw new ArgumentNullException("validationService");
            _validationService = validationService;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        [Route("")]
        public async Task<HttpResponseMessage> PostFile(
            long assetTypeId, long attributeId, long? assetId = null)
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var fileType = _fileService.GetAttributeMediaType(
                assetTypeId, attributeId);
            var uploadsDirRelPath = string.Empty;
            switch (fileType)
            {
                case Enumerators.MediaType.File:
                    uploadsDirRelPath = _fileService.GetDocsUploadDirectory(
                        assetTypeId, attributeId);
                    break;
                case Enumerators.MediaType.Image:
                    uploadsDirRelPath = _fileService.GetImagesUploadDirectory(
                        assetTypeId, attributeId);
                    break;
                case Enumerators.MediaType.ReportTemplate:
                    uploadsDirRelPath = _fileService.GetReportTemplatesUploadDirectory();
                    break;
            }

            var root = HostingEnvironment.MapPath(uploadsDirRelPath);
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            var provider = new MultipartFormDataStreamProvider(root);
            await request.Content.ReadAsMultipartAsync(provider);

            var fileData = provider.FileData.First();
            var fileName = fileData.Headers.ContentDisposition.FileName;
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                fileName = fileName.Trim('"');
            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                fileName = Path.GetFileName(fileName);

            var validationResult = _validationService.ValidateFileUpload(
                fileName, fileType);

            if (!validationResult.IsValid)
            {
                _logger.InfoFormat("Invalid upload: {0}", 
                    validationResult.GetErrorMessage());
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    validationResult.GetErrorMessage());
            }

            var destinationFilename = _fileService.GetDestinationFilename(
                fileName, uploadsDirRelPath);

            _logger.DebugFormat("Uploading {0} to {1}",
                fileData.LocalFileName, destinationFilename);

            File.Move(fileData.LocalFileName, destinationFilename);

            var result = new UploadResultModel
            {
                Filename = Path.GetFileName(destinationFilename)
            };

            if (fileType == Enumerators.MediaType.Image)
                result.ImageUrl = string.Format("/{0}/{1}",
                    uploadsDirRelPath.TrimStart(new[] { '~', '/' }),
                    Path.GetFileName(destinationFilename));

            return Request.CreateResponse(
                HttpStatusCode.OK,
                result);
        }
    }
}
