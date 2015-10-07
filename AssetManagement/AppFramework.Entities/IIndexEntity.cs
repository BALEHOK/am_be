namespace AppFramework.Entities
{
    public interface IIndexEntity : ISearchable
    {
        long OwnerId { get; set; }
        long? UserId { get; set; }
        long DepartmentId { get; set; }
        long LocationUid { get; set; }
        string Name { get; set; }
        string User { get; set; }
        string BarCode { get; set; }
        string Description { get; set; }
        string Department { get; set; }
        string Location { get; set; }
        string DisplayValues { get; set; }
        string DisplayExtValues { get; set; }
        string EntityConfigKeywords { get; set; }
        string Keywords { get; set; }
        string AllAttribValues { get; set; }
        string AllAttrib2IndexValues { get; set; }
        string AllContextAttribValues { get; set; }
        string TaxonomyKeywords { get; set; }
        string CategoryKeywords { get; set; }
        string Subtitle { get; set; }
    }
}
