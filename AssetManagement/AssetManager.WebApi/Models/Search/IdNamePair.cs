namespace AssetManager.WebApi.Models.Search
{
    public class IdNamePair<TId, TValue>
    {
        public TId Id { get; set; }
        public TValue Name { get; set; }
    }
}