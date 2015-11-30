using System;
using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models
{
    public class SearchResultModel
    {
        public Guid SearchId { get; set; }
        public List<IndexEntity> Entities { get; set; }
    }
}