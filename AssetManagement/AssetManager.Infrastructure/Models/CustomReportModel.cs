namespace AssetManager.Infrastructure.Models
{
    public class CustomReportModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public bool IsFinancial { get; set; }
        public string FileName { get; set; }
    }
}