using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using AppFramework.Core.Exceptions;
using AssetManager.Infrastructure.Models;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using System.Net.Http;
using Common.Logging;
using System.Collections.Generic;

namespace AssetManager.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IEnvironmentSettings _envSettings;
        private readonly IAttributeRepository _attributeRepository;
        
        private readonly ILog _logger;

        public FileService(
            IEnvironmentSettings environmentSettings,
            IAttributeRepository attributeRepository,
            ILog logger)
        {
            if (environmentSettings == null)
                throw new ArgumentNullException("environmentSettings");
            _envSettings = environmentSettings;
            if (attributeRepository == null)
                throw new ArgumentNullException("attributeRepository");
            _attributeRepository = attributeRepository;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        public Enumerators.MediaType GetAttributeMediaType(long assetTypeId, long attributeId)
        {
            var attribute = _attributeRepository.GetPublishedById(attributeId, a => a.DataType);
            if (attribute == null)
                throw new EntityNotFoundException("attribute");

            return attribute.DataType.Name == "image"
                ? Enumerators.MediaType.Image
                : Enumerators.MediaType.File;
        }

        public Enumerators.MediaType GetAttributeMediaType(AttributeModel attribute)
        {
            return attribute.Datatype == "image"
                ? Enumerators.MediaType.Image
                : Enumerators.MediaType.File;
        }

        public FileInfo UploadFile(IEnumerable<MultipartFileData> fileData, string uploadsDir)
        {
            if (fileData == null)
                throw new ArgumentNullException("multipartFileData");
            if (!fileData.Any())
                throw new ArgumentException("No file provided to upload");

            var sourceFile = fileData.First();
            var fileName = sourceFile.Headers.ContentDisposition.FileName;
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                fileName = fileName.Trim('"');
            if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                fileName = Path.GetFileName(fileName);

            var destinationFilename = _getDestinationFilename(
               fileName, uploadsDir);

            _logger.DebugFormat("Uploading {0} to {1}",
                sourceFile.LocalFileName, destinationFilename);

            File.Move(sourceFile.LocalFileName, destinationFilename);

            return new FileInfo(destinationFilename);
        }

        public FileInfo MoveFileOnAssetCreation(
            long assetTypeId,
            long attributeId,
            long relatedAssetTypeId,
            long relatedAttributeId,
            string filename)
        {
            var docsBaseDir = _envSettings.GetDocsUploadBaseDir();

            var sourceRelativePath = _envSettings.GetAssetMediaRelativePath(
                assetTypeId, attributeId);

            var sourceFilepath = HostingEnvironment.MapPath(
                Path.Combine(docsBaseDir, sourceRelativePath, filename));
                
            if (!File.Exists(sourceFilepath))
                throw new Exception("Cannot find uploaded file to create Document asset");

            var destRelativePath = _envSettings.GetAssetMediaRelativePath(
                relatedAssetTypeId, relatedAttributeId);
            var destAbsolutePath = HostingEnvironment.MapPath(
                Path.Combine(docsBaseDir, destRelativePath));
            var destFilepath = _getDestinationFilename(
                filename, destAbsolutePath);

            Directory.CreateDirectory(destAbsolutePath);
            File.Move(sourceFilepath, destFilepath);
            return new FileInfo(destFilepath);
        }

        public string GetFilepath(AttributeModel attribute)
        {
            var filepathOrFilename = attribute.Value.ToString();

            if (Path.GetFileName(filepathOrFilename) == filepathOrFilename)
            {
                var relPath = _envSettings.GetAssetMediaRelativePath(
                    attribute.AssetTypeId, attribute.Id);

                var baseDir = attribute.Datatype == "image"
                    ? _envSettings.GetImagesUploadBaseDir()
                    : _envSettings.GetDocsUploadBaseDir();

                if (!Path.IsPathRooted(baseDir))
                    baseDir = HostingEnvironment.MapPath(baseDir);

                return Path.Combine(baseDir, relPath, filepathOrFilename);
            }
            else
            {
                // support legacy database values containing full path to a file
                return HostingEnvironment.MapPath(filepathOrFilename);
            }
        }

        private string _getDestinationFilename(
            string uploadedFilename,
            string uploadsDir)
        {
            string fileName = Path.GetFileName(uploadedFilename);
            string fullFileName = Path.Combine(uploadsDir, fileName);
            int attempt = 1;
            while (File.Exists(fullFileName))
            {
                var tempFilename = Path.GetFileNameWithoutExtension(fullFileName);
                tempFilename = Regex.Replace(tempFilename, @"(\(\d+\))", string.Empty);
                fileName = string.Format("{0}({2}){1}",
                    tempFilename,
                    Path.GetExtension(fullFileName),
                    attempt);
                attempt++;
                fullFileName = Path.Combine(uploadsDir, fileName);
            }
            return fullFileName;
        }
    }
}
