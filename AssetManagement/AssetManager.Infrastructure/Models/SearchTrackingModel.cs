using AppFramework.Core.Classes.SearchEngine.Enumerations;

namespace AssetManager.Infrastructure.Models
{
    public class SearchTrackingModel
    {
        public SearchType SearchType { get; set; }
        public string VerboseString { get; set; }
    }
}