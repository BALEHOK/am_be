using System.Collections.Generic;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Models
{
    public class SearchResultModel
    {
        public long SearchId { get; set; }
        public List<IIndexEntity> Entities { get; set; }
    }
}