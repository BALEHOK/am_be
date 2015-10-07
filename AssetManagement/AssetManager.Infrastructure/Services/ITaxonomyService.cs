using AssetManager.Infrastructure.Models;
using System;

namespace AssetManager.Infrastructure.Services
{
    public interface ITaxonomyService
    {
        TaxonomyModel GetTaxonomyByAssetTypeId(long assetTypeId);
    }
}
