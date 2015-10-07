namespace AppFramework.DataProxy.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;
    using AppFramework.Entities;

    public class TaxonomyRepository : DataRepository<Taxonomy>
    {
        public TaxonomyRepository(ObjectContext context)
            : base(context)
        {

        }

        public IEnumerable<Taxonomy> GetWithAggregations(Expression<Func<Taxonomy, bool>> filter,
            Func<IQueryable<Taxonomy>, IOrderedQueryable<Taxonomy>> orderBy = null)
        {
            return Get(_objectSet
                .Include(d => d.TaxonomyItem                    
                    .Select(ti => ti.ParentItem)
                    .Select(ti => ti.ChildItems)),
                filter, orderBy);
        }

        public Taxonomy SingleOrDefaultWithAggregations(Expression<Func<Taxonomy, bool>> filter)
        {
            return SingleOrDefault(_objectSet
                .Include(d => d.TaxonomyItem
                    .Select(ti => ti.ParentItem)
                    .Select(ti => ti.ChildItems)),
                        filter);
        }

        public Taxonomy SingleWithAggregations(Expression<Func<Taxonomy, bool>> filter)
        {
            return Single(_objectSet
                .Include(d => d.TaxonomyItem
                    .Select(ti => ti.ParentItem)
                    .Select(ti => ti.ChildItems)),
                        filter);
        }
    }
}
