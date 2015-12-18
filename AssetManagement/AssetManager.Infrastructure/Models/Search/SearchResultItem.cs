using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models.Search
{
    public class SearchResultItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long DynEntityConfigId { get; set; }
        public long DynEntityId { get; set; }
        public long DynEntityUid { get; set; }
        public KeyValuePair<string, string>[] DisplayValues { get; set; }
        public KeyValuePair<string, string>[] DisplayExtValues { get; set; }
        public string AllAttribValues { get; set; }
        public string AllAttrib2IndexValues { get; set; }
        public string CategoryKeywords { get; set; }
    }
}