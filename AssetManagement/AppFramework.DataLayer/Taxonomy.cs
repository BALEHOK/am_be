//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppFramework.DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Taxonomy
    {
        public Taxonomy()
        {
            this.TaxonomyItem = new HashSet<TaxonomyItem>();
        }
    
        public long TaxonomyUid { get; set; }
        public int TaxonomyId { get; set; }
        public int Revision { get; set; }
        public bool ActiveVersion { get; set; }
        public string Name { get; set; }
        public string NameTranslationId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsCategory { get; set; }
        public bool IsDraft { get; set; }
        public long UpdateUserId { get; set; }
        public System.DateTime UpdateDate { get; set; }
    
        public virtual ICollection<TaxonomyItem> TaxonomyItem { get; set; }
    }
}
