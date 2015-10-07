namespace AppFramework.Reports.CustomReports
{
    public interface ICustomReport<out T>
    {
        long Id { get; }
        long AssetTypeId { get; }
        T ReportObject { get; }
        string Name { get; }
        string FileName { get; }
    }
}