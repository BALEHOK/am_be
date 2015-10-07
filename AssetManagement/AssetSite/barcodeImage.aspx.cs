using System;
using System.IO;
using Microsoft.Practices.Unity;
using AssetManager.Infrastructure.Services;

namespace AssetSite
{
    public partial class barcodeImage : BasePage
    {
        [Dependency]
        public IBarcodeService BarcodeService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var barcode = Request.QueryString["barcode"];
            if (barcode != null)
            {
                var imagePath = BarcodeService.GetBarcodeImageUrl(barcode);
                var imageFullPath = Server.MapPath(imagePath);
                var imageBytes = File.ReadAllBytes(imageFullPath);
                
                Response.ContentType = "image/png";
                Response.AddHeader("Content-Length", imageBytes.Length.ToString());
                Response.AddHeader("content-disposition", "attachment; filename=" + Guid.NewGuid().ToString() + ".png");

                Response.BinaryWrite(imageBytes);
                Response.End();
            }
        }
    }
}