using System;
namespace AppFramework.Entities
{

    public interface ISearchable
    {
        long DynEntityConfigId { get; set; }
        long DynEntityId { get; set; }
        long DynEntityUid { get; set; }
        long DynEntityConfigUid { get; set; }
        string TaxonomyItemsIds { get; set; }
        DateTime UpdateDate { get; set; }
    }
}
