using System.Collections.Generic;
using AppFramework.Core.Classes.Batch;

namespace AppFramework.Core.Classes.IE
{
    public interface IImportExportManager
    {
        BatchJob SynkAssets(string filePath, long assetTypeId, BindingInfo bindings, string synchronizationField, List<string> sheets, bool deleteSourceOnSuccess);
    }
}