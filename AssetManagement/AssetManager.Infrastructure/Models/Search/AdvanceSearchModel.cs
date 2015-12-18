using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Models.Search
{
    public class AdvanceSearchModel : SearchModelBase
    {
        public AdvanceSearchModel()
        {
            AssetTypeContext = TimePeriodForSearch.CurrentTime;
        }

        public IdNamePair<long, string> AssetType { get; set; }

        public TimePeriodForSearch AssetTypeContext { get; set; }

        public AttributeFilter[] Attributes { get; set; }

        public string Taxonomy { get; set; }

        public Enumerations.SearchOrder SortBy { get; set; }
    }
}