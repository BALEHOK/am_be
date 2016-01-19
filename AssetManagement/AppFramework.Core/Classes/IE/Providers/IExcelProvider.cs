using System.Collections.Generic;

namespace AppFramework.Core.Classes.IE.Providers
{
    public interface IExcelProvider
    {
        StatusInfo Status { get; set; }

        List<string> GetExcelSheetNames(string filepath);

        List<string> GetFields(string filepath, List<string> sheets);
    }
}