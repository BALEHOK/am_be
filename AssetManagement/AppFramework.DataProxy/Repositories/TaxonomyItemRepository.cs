using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using AppFramework.Entities;

namespace AppFramework.DataProxy.Repositories
{
    public class TaxonomyItemRepository : DataRepository<TaxonomyItem>, ITaxonomyItemRepository
    {
        public TaxonomyItemRepository(ObjectContext context)
            : base(context)
        {
            
        }

        public TaxonomyItem GeTaxonomyItembyId(long taxonomyItemId)
        {
            return AsQueryable().SingleOrDefault(ti => ti.TaxonomyItemId == taxonomyItemId && ti.ActiveVersion);
        }

        public List<TaxonomyItem> GetTaxonomyItemsByAssetTypeId(long assetTypeId)
        {
            var manyToMany = _context.CreateObjectSet<DynEntityConfigTaxonomy>();
            var taxonomyItems = AsQueryable();
            return (
                from relation in manyToMany
                from taxonomyItem in taxonomyItems
                where relation.DynEntityConfigId == assetTypeId
                      && relation.TaxonomyItemId == taxonomyItem.TaxonomyItemId
                      && taxonomyItem.ActiveVersion
                      && taxonomyItem != null
                select taxonomyItem
                )
                .Include(ti => ti.Taxonomy)
                .Include(ti => ti.ParentItem)
                .ToList();
        }
    }
}
