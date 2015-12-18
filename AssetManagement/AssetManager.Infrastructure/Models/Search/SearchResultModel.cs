using System;
using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models.Search
{
    public class SearchResultModel
    {
        public Guid SearchId { get; set; }
        public List<SearchResultItem> Entities { get; set; }
    }
}