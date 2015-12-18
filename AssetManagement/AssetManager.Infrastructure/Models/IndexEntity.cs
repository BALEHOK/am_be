using System;

namespace AssetManager.Infrastructure.Models
{
    public class IndexEntity
    {
        public long DynEntityConfigId { get; set; }
        public long DynEntityId { get; set; }
        public long DynEntityUid { get; set; }
        public object DisplayValues { get; set; }
        public object DisplayExtValues { get; set; }
        public string AllAttribValues { get; set; }
        public string AllAttrib2IndexValues { get; set; }
        public string CategoryKeywords { get; set; }
    }
}