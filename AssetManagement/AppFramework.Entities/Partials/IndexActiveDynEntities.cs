namespace AppFramework.Entities
{

    public partial class IndexActiveDynEntities : IIndexEntity
    {
        public int Rank { get; set; }
        public int AllAttribValuesRank { get; set; }
        public int AllContextAttribValuesRank { get; set; }
        public int AllAttrib2IndexValuesRank { get; set; }
        public string Subtitle { get; set; }
    }
}
