using System.Collections.Generic;
using AppFramework.Entities;

namespace AppFramework.DataProxy.Repositories
{
    public interface ITaxonomyItemRepository : IDataRepository<TaxonomyItem>
    {
        List<TaxonomyItem> GetTaxonomyItemsByAssetTypeId(long assetTypeId);
        TaxonomyItem GeTaxonomyItembyId(long taxonomyItemId);
    }
}