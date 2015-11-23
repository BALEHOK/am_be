using System;
using System.Collections.Generic;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Models
{
    public class SearchResultModel
    {
        public Guid SearchId { get; set; }
        public List<IIndexEntity> Entities { get; set; }
    }
}