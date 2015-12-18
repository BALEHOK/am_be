namespace AssetManager.Infrastructure.Models.Search
{
    public class SimpleSearchModel : SearchModelBase
    {
        public SimpleSearchModel()
        {
            Query = string.Empty;
            Context = 1;
        }

        private string _query;
        public string Query
        {
            get { return _query; }
            set { _query = value ?? string.Empty; }
        }

        public byte Context { get; set; }
        public long? AssetType { get; set; }
        public int? Taxonomy { get; set; }
        public byte? SortBy { get; set; }
        public long? AttributeId { get; set; }
        public long? AssetId { get; set; }
    }
}