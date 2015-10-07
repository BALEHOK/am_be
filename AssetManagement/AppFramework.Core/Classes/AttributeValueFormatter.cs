using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.DynLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Core.Classes
{
    public class AttributeValueFormatter : IAttributeValueFormatter
    {
        private readonly ILinkedEntityFinder _linkedEntityFinder;
        
        public AttributeValueFormatter(
            ILinkedEntityFinder linkedEntityFinder)
        {
            if (linkedEntityFinder == null)
                throw new ArgumentNullException("linkedEntityFinder");
            _linkedEntityFinder = linkedEntityFinder;
        }

        public string GetDisplayValue(
            AssetTypeAttribute attributeConfig, 
            object value, 
            bool isActiveVersion)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return string.Empty;

            if (attributeConfig.DataTypeEnum != Enumerators.DataType.DynList &&
                attributeConfig.DataTypeEnum != Enumerators.DataType.DynLists)
            {
                if (value == null)
                    throw new ArgumentNullException("value cannot be null");
                if (value.ToString() == string.Empty)
                    throw new ArgumentException("value cannot be empty string");
            }

            string result = string.Empty;
            switch (attributeConfig.DataTypeEnum)
            {
                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Document:
                    long assetId;
                    if (long.TryParse(value.ToString(), out assetId) && assetId > 0)
                    {
                        var assetTypeId = attributeConfig.RelatedAssetTypeID.Value;
                        var assetTypeAttributeId = attributeConfig.RelatedAssetTypeAttributeID.Value;
                        result = _linkedEntityFinder.GetRelatedAssetDisplayName(
                            assetTypeId,
                            assetTypeAttributeId,
                            assetId,
                            isActiveVersion);
                    }
                    break;

                case Enumerators.DataType.Role:
                    PredefinedRoles role = (PredefinedRoles)Convert.ToInt32(value);
                    result = role.ToString();
                    break;

                case Enumerators.DataType.DynList:
                case Enumerators.DataType.DynLists:
                    result = null;
                    //throw new NotSupportedException("Please use second overload of GetDisplayValue method");    
                    break;

                default:
                    result = value.ToString();
                    break;
            }
            return result;
        }

        public string GetDisplayValue(IEnumerable<DynamicListValue> listValues)
        {
            return string.Join(
              " ",
              listValues.OrderByDescending(v => v.DisplayOrder)
                              .Select(a => a.Value)
                              .Where(a => !string.IsNullOrEmpty(a)));
        }
    }
}
