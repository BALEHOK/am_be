namespace AppFramework.DataProxy.Repositories
{
    using System.Data.Objects;
    using AppFramework.Entities;

    public class DynEntityConfigRepository : DataRepository<DynEntityConfig>
    {
        public DynEntityConfigRepository(ObjectContext context)
            : base(context)
        {

        }

        //public DynEntityConfig SingleOrDefaultWithAggregations(Expression<Func<DynEntityConfig, bool>> filter)
        //{
        //    return SingleOrDefault(_objectSet.Include(d => d.AttributePanel
        //                                        .Select(ap => ap.AttributePanelAttribute
        //                                            .Select(apa => apa.DynEntityAttribConfig)))
        //                                     .Include(d => d.DynEntityAttribConfig
        //                                         .Select(deac => deac.DataType)),
        //                           filter);
        //}

        //public DynEntityConfig SingleOrDefaultWithAggregationsAndValidation(Expression<Func<DynEntityConfig, bool>> filter)
        //{
        //    return SingleOrDefault(_objectSet.Include(d => d.AttributePanel
        //                                        .Select(ap => ap.AttributePanelAttribute
        //                                            .Select(apa => apa.DynEntityAttribConfig)))
        //                                     .Include(d => d.DynEntityAttribConfig
        //                                         .Select(deac => deac.DataType))
        //                                     .Include(d => d.DynEntityAttribConfig
        //                                         .Select(deac => deac.DataType.ValidationList
        //                                             .Select(vl => vl.ValidationOperator.ValidationOperand)))
        //                                     .Include(d => d.DynEntityAttribConfig
        //                                         .Select(deac => deac.DataType.ValidationOperand)),
        //                           filter);
        //}
    }
}
