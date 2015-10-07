using AppFramework.Core.Classes.DynLists;
using System;
using System.Collections.Generic;
namespace AppFramework.Core.Classes
{
    public interface IAttributeValueFormatter
    {
        string GetDisplayValue(AssetTypeAttribute attributeConfig, object value, bool isActiveVersion);
        string GetDisplayValue(IEnumerable<DynamicListValue> listValues);
    }
}
