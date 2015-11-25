using System;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;

namespace AssetManager.WebApi.Models.Search
{
    public class AdvanceSearchModel
    {
        public IdNamePair<long, string> AssetType { get; set; }

        public TimePeriodForSearch AssetTypeContext { get; set; }

        public AttributeFilter[] Attributes { get; set; }

        public Guid SearchId { get; set; }

        public string Taxonomy { get; set; }

        public int Page { get; set; }

        public Enumerations.SearchOrder SortBy { get; set; }

        public AdvanceSearchModel()
        {
            Page = 1;
            AssetTypeContext = TimePeriodForSearch.CurrentTime;
        }
    }
}