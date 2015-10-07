using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Exceptions;

namespace AppFramework.Core.Classes.IE.Adapters
{
    public class XmlConverterBase
    {
        /// <summary>
        /// Gets the status of performed operations
        /// </summary>
        private readonly Dictionary<long, AssetTypeAttribute> _assetAttributeTypesHash = new Dictionary<long, AssetTypeAttribute>();

        protected readonly XNamespace _namespace = "http://tempuri.org/AssetManagementAssets.xsd";

        /// <summary>
        /// Returns the Value subnode for Attribute node
        /// </summary>
        /// <returns></returns>
        protected XElement GetValueSubNode(AssetTypeAttribute attribute, string nodeValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(nodeValue) && 
                string.IsNullOrEmpty(defaultValue) && 
                (attribute.IsRequired && attribute.Editable))
                throw new ImportException(string.Format("Required attribute missing: {0}", attribute.Name));

            XElement node = null;
            string value = string.IsNullOrEmpty(nodeValue)
                ? defaultValue
                : nodeValue;

            switch (attribute.DataTypeEnum)
            {
                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Document:
                    node = new XElement(XName.Get("Value", _namespace.NamespaceName),
                        value == "0" ? string.Empty : value);
                    break;

                case Enumerators.DataType.Assets:
                    node = new XElement(XName.Get("MultipleAssets", _namespace.NamespaceName),
                        ParseMultipleAssets(attribute, value));
                    break;

                case Enumerators.DataType.DynList:
                case Enumerators.DataType.DynLists:
                    node = new XElement(XName.Get("DynamicLists", _namespace.NamespaceName),
                        ParseDynListItems(attribute, value));
                    break;

                case Enumerators.DataType.Barcode:
                    var provider = new DefaultBarcodeProvider();
                    if (value == provider.DefaultValue)
                        value = provider.GenerateBarcode();
                    node = new XElement(XName.Get("Value", _namespace.NamespaceName), value);
                    break;
                case Enumerators.DataType.Permission:
                    node = new XElement(XName.Get("Value", _namespace.NamespaceName),
                        ParseEnum(attribute, value));
                    break;
                default:
                    node = new XElement(XName.Get("Value", _namespace.NamespaceName), value);
                    break;
            }
            return node;
        }

        private string ParseEnum(AssetTypeAttribute attribute, string value)
        {
            Permission parsed;
            if (PermissionsProvider.TryParse(value, out parsed))
                return parsed.GetCode().ToString();
            return "0";
        }

        private IEnumerable<XElement> ParseMultipleAssets(AssetTypeAttribute attribute, string value)
        {
            if (!_assetAttributeTypesHash.ContainsKey(attribute.ID))
                _assetAttributeTypesHash.Add(attribute.ID, attribute);
            attribute = _assetAttributeTypesHash[attribute.ID];

            string[] items = value.Split(new char[] { ';' });
            foreach (string item in items.Where(i => !string.IsNullOrWhiteSpace(i)))
            {
                XElement content;
                long assetId;
                //it can be value then lookup Id by Value
                content = !long.TryParse(item, out assetId) 
                    ? new XElement(XName.Get("Value", _namespace.NamespaceName), item) 
                    : new XElement(XName.Get("ID", _namespace.NamespaceName), assetId);
                yield return new XElement(XName.Get("MultipleAsset", _namespace.NamespaceName), content);
            }
        }

        /// <summary>
        /// TODO : retrieve a chain of lists
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private IEnumerable<XElement> ParseDynListItems(AssetTypeAttribute attribute, string value)
        {
            if (!_assetAttributeTypesHash.ContainsKey(attribute.ID))
                _assetAttributeTypesHash.Add(attribute.ID, attribute);
            attribute = _assetAttributeTypesHash[attribute.ID];

            string[] items = value.Split(new char[] { ',' });
            foreach (string item in items)
            {
                //value might be Id
                DynamicListItem listItem;
                attribute.DynamicList.TryParse(item.Trim(), out listItem);

                if (listItem == null)
                    continue;

                yield return new XElement(XName.Get("DynList", _namespace.NamespaceName),
                    new XElement(XName.Get("DynListId", _namespace.NamespaceName), listItem.ParentDynList != null ? listItem.ParentDynList.UID : 0),
                    new XElement(XName.Get("ParentListId", _namespace.NamespaceName), listItem.ParentDynList != null ? listItem.ParentDynList.UID : 0),
                    new XElement(XName.Get("Value", _namespace.NamespaceName), listItem.Value),
                    new XElement(XName.Get("DynListItemId", _namespace.NamespaceName), listItem.ID));
            }
        }
    }
}