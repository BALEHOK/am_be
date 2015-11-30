using System;

namespace AssetManager.Infrastructure.Models
{
    public class IndexEntity
    {
        public long DynEntityConfigId { get; set; }
        public long DynEntityId { get; set; }
        public long DynEntityUid { get; set; }
        public long DynEntityConfigUid { get; set; }
        public string TaxonomyItemsIds { get; set; }
        public DateTime UpdateDate { get; set; }
        public long OwnerId { get; set; }
        public long? UserId { get; set; }
        public long DepartmentId { get; set; }
        public long LocationUid { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public object DisplayValues { get; set; }
        public object DisplayExtValues { get; set; }
        public string EntityConfigKeywords { get; set; }
        public string Keywords { get; set; }
        public string AllAttribValues { get; set; }
        public string AllAttrib2IndexValues { get; set; }
        public string AllContextAttribValues { get; set; }
        public string TaxonomyKeywords { get; set; }
        public string CategoryKeywords { get; set; }
        public string Subtitle { get; set; }
    }
}