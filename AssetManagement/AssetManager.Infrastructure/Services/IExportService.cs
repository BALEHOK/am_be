using AppFramework.Entities;

namespace AssetManager.Infrastructure.Services
{
    public interface IExportService
    {
        byte[] ExportToXml(SearchTracking searchTracking, long userId);
        byte[] ExportToExcel(SearchTracking searchTracking, long userId);
        byte[] ExportToTxt(SearchTracking searchTracking, long userId);
    }
}