using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class BarcodeModel
    {
        public string Barcode { get; set; }
        public string Base64Image { get; set; }
    }
}