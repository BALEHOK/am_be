using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.DynLists;

namespace AppFramework.Core.Classes.IE.Adapters
{
    internal struct ExportInfo
    {
        public string AssetTypeName;
        public long AssetTypeId;
        public XDocument Xml;
    }

    /// <summary>
    /// Converts collection of assets to an XML 
    /// </summary>
    internal class AssetsToXmlConverter : XmlConverterBase
    {
        private readonly List<Asset> _assets;

        public AssetsToXmlConverter(IEnumerable<Asset> assets)
        {
            _assets = assets.ToList();
        }

        /// <summary>
        /// Converts collection of any assets to XML
        /// </summary>
        /// <returns>AT ID + XML</returns>
        public ActionResult<List<ExportInfo>> GetXML()
        {
            ActionResult<List<ExportInfo>> result = new ActionResult<List<ExportInfo>>();

            // group by AT and export types separately
            foreach (IGrouping<long, Asset> assets in _assets.GroupBy(a => a.GetConfiguration().ID))
            {
                XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                IEnumerable<XElement> xassets = ConvertAssetsToXElements(assets.ToList());
                document.Add(new XElement(XName.Get("Assets", _namespace.NamespaceName), xassets));
                result.Data.Add(
                    new ExportInfo()
                    {
                        AssetTypeId = assets.Key,
                        AssetTypeName = assets.First().GetConfiguration().NameInvariant,
                        Xml = document
                    });
            }
            return result;
        }

        /// <summary>
        /// Converts assets to XElements
        /// </summary>
        /// <param name="assets"></param>
        /// <returns></returns>
        private IEnumerable<XElement> ConvertAssetsToXElements(IEnumerable<Asset> assets)
        {
            foreach (Asset asset in assets)
            {
                yield return new XElement(XName.Get("Asset", _namespace.NamespaceName), GetInnerNodes(asset));
            }
        }

        /// <summary>
        /// Gets inner nodes for XML asset element
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private IEnumerable<XElement> GetInnerNodes(Asset asset)
        {
            // ID node
            yield return new XElement(XName.Get("ID", _namespace.NamespaceName), asset.ID);
            // attributes node
            yield return new XElement(XName.Get("Attributes", _namespace.NamespaceName), GetAttributesNodes(asset));
        }

        /// <summary>
        /// Gets the collection of attributes nodes for asset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private IEnumerable<XElement> GetAttributesNodes(Asset asset)
        {
            foreach (var attribute in asset.Attributes.Where(a => a.GetConfiguration().IsShownOnPanel))
            {
                var attributeConfig = attribute.GetConfiguration();
                var relatedId = string.Empty;
                if (attributeConfig.DataTypeEnum == Enumerators.DataType.Asset)
                    relatedId = attributeConfig.RelatedAssetTypeAttributeID.ToString();

                yield return new XElement(XName.Get("Attribute", _namespace.NamespaceName),
                    new XElement(XName.Get("Id", _namespace.NamespaceName), attributeConfig.ID),
                    new XElement(XName.Get("Name", _namespace.NamespaceName), XmlConvert.DecodeName(attributeConfig.Name)),
                    GetValueSubNode(attributeConfig, attribute.Value, string.Empty),
                    new XElement(XName.Get("RelatedAttributeId", _namespace.NamespaceName), relatedId));
            }
        }
    }
}
