using AssetManager.Infrastructure.Services;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/barcode")]
    public class BarcodeController : ApiController
    {
        private readonly IBarcodeService _barcodeService;

        public BarcodeController(IBarcodeService barcodeService)
        {
            if (barcodeService == null)
                throw new ArgumentNullException("barcodeService");
            _barcodeService = barcodeService;
        }

        [Route(""), HttpGet]
        public BarcodeModel GenerateBarcode()
        {
            var barcode = _barcodeService.GenerateBarcode();
            return new BarcodeModel
            {
                Barcode = barcode,
                Base64Image = GetBarcodeImageContent(barcode)
            };
        }

        [Route("{barcode}"), HttpGet]
        [CacheOutput(ServerTimeSpan = 60 * 60 * 24, ClientTimeSpan = 60 * 60 * 24)]
        public string GetBarcodeImageContent(string barcode)
        {
            if (!_barcodeService.ValidateBarcode(barcode))
                throw new HttpResponseException(
                    new HttpResponseMessage(HttpStatusCode.BadRequest) 
                    { 
                        ReasonPhrase = "Given parameter is not valid barcode." 
                    });
            var relPath = _barcodeService.GetBarcodeImageUrl(barcode);
            var bytes = System.IO.File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath(relPath));
            return Convert.ToBase64String(bytes);
        }
    }
}