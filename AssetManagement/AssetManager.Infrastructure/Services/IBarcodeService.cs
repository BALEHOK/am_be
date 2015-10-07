using System;

namespace AssetManager.Infrastructure.Services
{
    public interface IBarcodeService
    {
        string GetBarcodeImageUrl(string barcode);

        bool ValidateBarcode(string barcode);

        string GenerateBarcode();
    }
}
