using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Services
{
    public interface ITaxonomyService
    {
        TaxonomyModel GetTaxonomyByAssetTypeId(long assetTypeId);
    }
}