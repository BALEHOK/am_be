using System;
namespace AssetManager.Infrastructure.Services
{
    public interface IExportService
    {
        string ExportSearchResultToHtml(System.Collections.Generic.IEnumerable<AppFramework.Entities.IIndexEntity> searchResults);
        string ExportSearchResultToTxt(System.Collections.Generic.IEnumerable<AppFramework.Entities.IIndexEntity> searchResults);
        string ExportSearchResultToXml(System.Collections.Generic.IEnumerable<AppFramework.Entities.IIndexEntity> searchResults);
        byte[] ExportSearchResultToExcel(System.Collections.Generic.IEnumerable<AppFramework.Entities.IIndexEntity> searchResults);
    }
}
