using System.Collections.Generic;
using System.Xml.Linq;

namespace AppFramework.Core.Classes.IE.Adapters
{
    public interface IExcelToXmlConverter
    {
        /// <summary>
        /// Returns the XML document, which contains information about AD users
        /// </summary>
        /// <returns></returns>
        XDocument ConvertToXml(string filePath, BindingInfo bindings, AssetType at, List<string> sheets);
    }
}