using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AssetManager.Infrastructure.Models;
using AppFramework.ConstantsEnumerators;

namespace AssetManager.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IUnitOfWork _unitOfWork;

        public FileService(
            IEnvironmentSettings environmentSettings,
            IUnitOfWork unitOfWork)
        {
            if (environmentSettings == null)
                throw new ArgumentNullException("environmentSettings");
            _environmentSettings = environmentSettings;
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;            
        }

        public Enumerators.MediaType GetAttributeMediaType(long assetTypeId, long attributeId)
        {
            if (assetTypeId == 0 && attributeId == 0)
                return Enumerators.MediaType.ReportTemplate;

            var attribute = _unitOfWork.DynEntityAttribConfigRepository
            .SingleOrDefault(d => d.DynEntityAttribConfigId == attributeId && d.ActiveVersion,
                i => i.DataType);
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

        public virtual string GetImagesUploadDirectory(long assetTypeId, long attributeId)
        {
            return _environmentSettings.GetImagesUploadDirectory(assetTypeId, attributeId);
        }

        public string GetReportTemplatesUploadDirectory()
        {            
            return Path.Combine("~/App_Data/uploads", "report_templates");
        }
        
        public virtual string GetDocsUploadDirectory(long assetTypeId, long attributeId)
        {            
            return _environmentSettings.GetDocsUploadDirectory(assetTypeId, attributeId); 
        }

        public string GetRelativeFilepath(
            long assetTypeId,
            long attributeId,
            string datatype,
            string value)
        {
            string uploadsDirRelPath;
            var fileType = datatype == "image"
                ? Enumerators.MediaType.Image
                : Enumerators.MediaType.File;

            if (fileType == Enumerators.MediaType.Image)
                uploadsDirRelPath = GetImagesUploadDirectory(assetTypeId, attributeId);
            else
                uploadsDirRelPath = GetDocsUploadDirectory(assetTypeId, attributeId);

            return string.Format("{0}/{1}", uploadsDirRelPath, value);
        }

        public string GetDestinationFilename(
            string uploadedFilename,
            string uploadsDirRelPath)
        {
            string fileName = Path.GetFileName(uploadedFilename);
            string uploadsDirFullpath = HostingEnvironment.MapPath(uploadsDirRelPath);
            string fullFileName = Path.Combine(uploadsDirFullpath, fileName);
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
                fullFileName = Path.Combine(uploadsDirFullpath, fileName);
            }
            return fullFileName;
        }

        public string MoveFileOnAssetCreation(
            long assetTypeId,
            long attributeId,
            long relatedAssetTypeId,
            long relatedAttributeId,
            string filename)
        {
            var initialDir = GetDocsUploadDirectory(
                assetTypeId, attributeId);
            var initialFilepath = Path.Combine(HostingEnvironment.MapPath(initialDir), filename);
            if (!File.Exists(initialFilepath))
                throw new Exception("Cannot find uploaded file to create Document asset");

            var destinationDir = GetDocsUploadDirectory(
                relatedAssetTypeId, relatedAttributeId);
            var destinationFilepath = GetDestinationFilename(
                filename, destinationDir);

            Directory.CreateDirectory(HostingEnvironment.MapPath(destinationDir));
            File.Move(initialFilepath, destinationFilepath);

            return destinationFilepath;
        }
    }
}
