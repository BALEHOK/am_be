using AppFramework.Entities;
using System;
using System.Collections.Generic;

namespace AppFramework.DataProxy.Repositories
{
    public interface ITaxonomyItemRepository : IDataRepository<TaxonomyItem>
    {
        IEnumerable<TaxonomyItem> GetTaxonomyItemsByAssetTypeId(long assetTypeId);
    }
}
