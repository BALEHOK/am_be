using System;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine
{
    public class SearchCounter : ISearchCounter
    {
        public int? Count { get; set; }

        public long? id { get; set; }

        public Guid? SearchId { get; set; }

        public string Type { get; set; }

        public long? UserId { get; set; }

        public string Name { get; set; }
    }
}
