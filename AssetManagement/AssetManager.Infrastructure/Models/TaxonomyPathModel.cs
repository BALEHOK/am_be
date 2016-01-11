namespace AssetManager.Infrastructure.Models
{
    public class TaxonomyPathModel
    {
        public string Name { get; set; }

        public TaxonomyPathModel Child { get; set; }
    }
}