namespace AssetManager.Infrastructure.Models.Search
{
    public class IdNamePair<TId, TName>
    {
        public IdNamePair()
        {
            
        }

        public IdNamePair(TId id, TName name)
        {
            Id = id;
            Name = name;
        }

        public TId Id { get; set; }
        public TName Name { get; set; }
    }
}