using AppFramework.Core.Classes;
using System.Collections.Generic;

namespace AssetManager.Infrastructure.Services
{
    public interface IFaqService
    {
        IEnumerable<Asset> GetFaqItems(System.Globalization.CultureInfo culture = null, int itemsNumber = 1000);

        long GetFaqAssetTypeId();
    }
}
