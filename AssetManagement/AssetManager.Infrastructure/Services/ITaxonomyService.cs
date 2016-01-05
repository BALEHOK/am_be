using System.Collections.Generic;
using AssetManager.Infrastructure.Models;

namespace AssetManager.Infrastructure.Services
{
    public interface ITaxonomyService
    {
        IEnumerable<TaxonomyModel> GetTaxonomyByAssetTypeId(long assetTypeId);
    }
}