using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Hosting;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Services
{
    public class BannerService : IBannerService
    {
        private const string LogoId = "logo";
        private const string BannerId = "banner";

        private readonly IFileService _fileService;
        private readonly IEnvironmentSettings _envSettings;
        private readonly IUnitOfWork _unitOfWork;

        private static BannerImages _bannerImages;
        private static string _uploadsDir;

        private BannerImages BannerImages
        {
            get
            {
                if (_bannerImages == null)
                {
                    var images = _unitOfWork.BannerImageRepository.Get();
                    var logoImage = images.SingleOrDefault(i => string.Equals(i.Id, LogoId));
                    var bannerImage = images.SingleOrDefault(i => string.Equals(i.Id, BannerId));
                    _bannerImages = new BannerImages
                    {
                        Logo = logoImage != null ? GetImageUrl(logoImage.Name) : string.Empty,
                        Banner = bannerImage != null ? GetImageUrl(bannerImage.Name) : string.Empty
                    };
                }

                return _bannerImages;
            }
        }

        private string UploadsDir
        {
            get
            {
                if (string.IsNullOrEmpty(_uploadsDir))
                {
                    _uploadsDir = _envSettings.GetBannerUploadBaseDir() + "/" + _envSettings.GetBannerRelativePath();

                    if (string.IsNullOrEmpty(_uploadsDir))
                    {
                        throw new ConfigurationErrorsException("Banner upload directory path can't be empty. Check configuration settings.");
                    }

                    if (!Path.IsPathRooted(_uploadsDir))
                    {
                        _uploadsDir = HostingEnvironment.MapPath(_uploadsDir);
                    }

                    if (!Directory.Exists(_uploadsDir))
                    {
                        Directory.CreateDirectory(_uploadsDir);
                    }
                }

                return _uploadsDir;
            }
        }

        public BannerService(IFileService fileService,
            IEnvironmentSettings envSettings,
            IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _envSettings = envSettings;
            _unitOfWork = unitOfWork;
        }

        public BannerImages GetImageUrls()
        {
            return BannerImages;
        }

        public string GetBannerUploadDir()
        {
            return UploadsDir;
        }

        public string UploadLogo(Collection<MultipartFileData> fileData)
        {
            var logoName = SaveImage(fileData, UploadsDir, LogoId);
            var url = GetImageUrl(logoName);
            BannerImages.Logo = url;
            return url;
        }

        public string UploadBanner(Collection<MultipartFileData> fileData)
        {
            var bannerName = SaveImage(fileData, UploadsDir, BannerId);
            var url = GetImageUrl(bannerName);
            BannerImages.Banner = url;
            return url;
        }

        private string SaveImage(Collection<MultipartFileData> fileData, string uploadsDir, string imageId)
        {
            var destinationFileName = _fileService.UploadFile(fileData, uploadsDir).Name;

            var entity = _unitOfWork.BannerImageRepository.SingleOrDefault(i => i.Id == imageId);
            if (entity != null)
            {
                entity.Name = destinationFileName;
                _unitOfWork.BannerImageRepository.Update(entity);
            }
            else
            {
                entity = new BannerImage { Id = imageId, Name = destinationFileName };
                _unitOfWork.BannerImageRepository.Insert(entity);
            }

            _unitOfWork.Commit();

            return destinationFileName;
        }

        private string GetImageUrl(string fileName)
        {
            var url = string.Format("{0}/{1}/{2}",
                _envSettings.GetAssetMediaHttpRoot(),
                _envSettings.GetBannerRelativePath(),
                fileName);
            return url;
        }
    }
}