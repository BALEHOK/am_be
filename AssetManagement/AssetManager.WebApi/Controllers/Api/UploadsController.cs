using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using AppFramework.ConstantsEnumerators;
using Common.Logging;
using AssetManager.Infrastructure;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/uploads")]
    public class UploadsController : ApiController
    {
        private readonly IFileService _fileService;
        private readonly IValidationService _validationService;
        private readonly ILog _logger;
        private readonly IEnvironmentSettings _envSettings;

        public UploadsController(
            IFileService fileService,
            IEnvironmentSettings envSettings,
            IValidationService validationService,
            ILog logger)
        {
            if (fileService == null)
                throw new ArgumentNullException("fileService");
            _fileService = fileService;
            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _envSettings = envSettings;
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
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(
                    HttpStatusCode.UnsupportedMediaType);

            var fileType = _fileService.GetAttributeMediaType(
                assetTypeId, attributeId);

            var relativePath = _envSettings.GetAssetMediaRelativePath(
                assetTypeId, attributeId);

            var uploadsDir = string.Format("{0}/{1}",
                fileType == Enumerators.MediaType.File
                    ? _envSettings.GetDocsUploadBaseDir()
                    : _envSettings.GetImagesUploadBaseDir(),
                relativePath);
                  
            if (!Path.IsPathRooted(uploadsDir))
                uploadsDir = HostingEnvironment.MapPath(uploadsDir);

            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var provider = new MultipartFormDataStreamProvider(uploadsDir);
            await Request.Content.ReadAsMultipartAsync(provider);

            var destinationFile = _fileService.UploadFile(provider.FileData, uploadsDir);

            var validationResult = _validationService.ValidateFileUpload(
                destinationFile.Name, fileType);

            if (!validationResult.IsValid)
            {
                destinationFile.Delete();

                _logger.InfoFormat("Invalid upload: {0}",
                    validationResult.GetErrorMessage());

                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    validationResult.GetErrorMessage());
            }

            return Request.CreateResponse(
                HttpStatusCode.OK,
                new UploadResultModel
                {
                    Filename = destinationFile.Name,
                    ImageUrl = fileType == Enumerators.MediaType.Image
                        ? string.Format("{0}/{1}/{2}", 
                            _envSettings.GetAssetMediaHttpRoot(), relativePath, destinationFile.Name)
                        : string.Empty
                });
        }
    }
}
