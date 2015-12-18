using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models.Search
{
    public class SearchCountersModel
    {
        public int TotalCount { get; set; }

        public List<SearchCounterModel> AssetTypes { get; set; }

        public List<SearchCounterModel> Taxonomies { get; set; }
    }

    public class SearchCounterModel
    {
        public int Count { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
    }
}
