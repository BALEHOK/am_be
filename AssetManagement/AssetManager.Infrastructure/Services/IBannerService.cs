using System.Collections.ObjectModel;
using System.Net.Http;
using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Services
{
    public interface IBannerService
    {
        BannerImages GetImageUrls();
        string UploadLogo(Collection<MultipartFileData> fileData);
        string UploadBanner(Collection<MultipartFileData> fileData);
        string GetBannerUploadDir();
    }
}