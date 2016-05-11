using System;
namespace AppFramework.Core.Classes.IE
{
    public interface IAssetsExporter
    {
        System.Data.DataTable ExportToDataTable(long assetTypeUid, System.Collections.Generic.List<SearchEngine.TypeSearchElements.AttributeElement> filter, long userId);
    }
}
