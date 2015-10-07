using AppFramework.Core.Classes.Barcode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;

namespace AssetManager.Infrastructure.Services
{
    public class BarcodeService : IBarcodeService
    {
        private readonly IBarcodeProvider _barcodeProvider;
        //private const string CachePath = "~/App_Data/cache";
        private readonly IEnvironmentSettings _environmentSettings;

        public BarcodeService(
            IEnvironmentSettings envSettings,
            IBarcodeProvider barcodeProvider)
        {
            if (envSettings == null)
                throw new ArgumentNullException("envSettings");
            _environmentSettings = envSettings;
            if (barcodeProvider == null)
                throw new ArgumentNullException("barcodeProvider");
            _barcodeProvider = barcodeProvider;
        }

        public bool ValidateBarcode(string barcode)
        {
            return _barcodeProvider.ValidateBarcode(barcode);
        }

        public string GenerateBarcode()
        {
            return _barcodeProvider.GenerateBarcode();
        }

        public string GetBarcodeImageUrl(string barcode)
        {
            var cachePath = _environmentSettings.GetCacheDirectory();
            var directory = HostingEnvironment.MapPath(cachePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var imagePath = string.Format("{0}/{1}.png", cachePath, barcode);
            var imageFullPath = HostingEnvironment.MapPath(imagePath);

            if (!File.Exists(imageFullPath))
            {
                var image = _barcodeProvider.GenerateBarcodeImage(barcode);
                image.Save(imageFullPath);
            }

            return imagePath;
        }
    }
}