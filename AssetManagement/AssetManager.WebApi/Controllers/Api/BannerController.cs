using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;
using AppFramework.Core.Classes;
using AppFramework.Core.Exceptions;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Services;

namespace AssetManager.WebApi.Controllers.Api
{
    /// <summary>
    /// Get and set app Logo and Banner images
    /// </summary>
    [RoutePrefix("api/logo-banner")]
    public class BannerController : ApiController
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        [Route("")]
        public object GetImageUrls()
        {
            return _bannerService.GetImageUrls();
        }

        [HttpPost]
        [Route("logo")]
        public async Task<HttpResponseMessage> UploadLogo()
        {
            ValidateRequest();

            var fileData = await GetFileData();

            var logoUrl = _bannerService.UploadLogo(fileData);

            return Request.CreateResponse(
                HttpStatusCode.OK,
                new UploadResultModel
                {
                    ImageUrl = logoUrl
                });
        }

        [HttpPost]
        [Route("banner")]
        public async Task<HttpResponseMessage> UploadBanner()
        {
            ValidateRequest();

            var fileData = await GetFileData();

            var bannerUrl = _bannerService.UploadBanner(fileData);

            return Request.CreateResponse(
                HttpStatusCode.OK,
                new UploadResultModel
                {
                    ImageUrl = bannerUrl
                });
        }

        private void ValidateRequest()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var user = Membership.GetUser(User.GetId()) as AssetUser;
            if (user == null || !user.IsAdministrator)
            {
                throw new InsufficientPermissionsException();
            }
        }

        private async Task<Collection<MultipartFileData>> GetFileData()
        {
            var uploadsDir = _bannerService.GetBannerUploadDir();
            var provider = new MultipartFormDataStreamProvider(uploadsDir);
            await Request.Content.ReadAsMultipartAsync(provider);
            return provider.FileData;
        }
    }
}