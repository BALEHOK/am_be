using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;
using Newtonsoft.Json;

namespace AssetManager.WebApi.Models.Search
{
    public class AdvanceSearchModel
    {
        [JsonProperty(PropertyName = "assetType")]
        public long AssetTypeId { get; set; }

        public TimePeriodForSearch Context { get; set; }

        [JsonProperty(PropertyName = "attribs")]
        public AttributeFilter[] Attributes { get; set; }

        public int SearchId { get; set; }

        public string Taxonomy { get; set; }

        public int Page { get; set; }

        public Enumerations.SearchOrder SortBy { get; set; }

        public AdvanceSearchModel()
        {
            Page = 1;
            Context = TimePeriodForSearch.CurrentTime;
        }
    }
}