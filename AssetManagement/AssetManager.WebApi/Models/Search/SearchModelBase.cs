using System;

namespace AssetManager.WebApi.Models.Search
{
    public class SearchModelBase
    {
        public SearchModelBase()
        {
            Page = 1;
        }

        public int Page { get; set; }

        public Guid? SearchId { get; set; }
    }
}