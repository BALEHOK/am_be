using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Classes.IE.Providers;
using AppFramework.Core.Exceptions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AppFramework.Core.Classes.IE.Adapters
{
    public class ExcelToXmlConverter : XmlConverterBase, IExcelToXmlConverter
    {
        private readonly ExcelProvider _excelProvider;

        public ExcelToXmlConverter()
        {
            _excelProvider = new ExcelProvider();
        }

        /// <summary>
        /// Returns the XML document, which contains information about AD users
        /// </summary>
        /// <returns></returns>
        public XDocument ConvertToXml(string filePath, BindingInfo bindings, AssetType at, IEnumerable<string> sheets)
        {
            // get DataSet from excel file
            var dataSet = _excelProvider.GetDataSet(filePath, sheets).Data;
            var document = new XDocument(new XDeclaration("1.0", "utf-16", "yes"));
            document.Add(new XElement(XName.Get("Assets", _namespace.NamespaceName), GetAssets(dataSet, at, bindings)));
            return document;
        }

        /// <summary>
        /// Returns the collection of XElements which represents assets
        /// </summary>
        /// <returns></returns>
        private IEnumerable<XElement> GetAssets(DataSet dataSet, AssetType at, BindingInfo bindings)
        {
            dataSet.Namespace = _namespace.NamespaceName;
            var xassets = new List<XElement>();
            foreach (DataTable table in dataSet.Tables)
            {
                if (table.Rows.Count > 0)
                {
                    var tmpSet = new DataSet("Assets") { Namespace = _namespace.NamespaceName };
                    tmpSet.Tables.Add(table.Copy());
                    tmpSet.Tables[0].TableName = "Asset";
                    xassets.AddRange(TransformDataSetToItemsCollection(tmpSet, at, bindings));
                }
            }
            return xassets;
        }

        /// <summary>
        /// Performs the DataSet transformation accordingly schema structure
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerable<XElement> TransformDataSetToItemsCollection(DataSet data, AssetType at, BindingInfo bindings)
        {
            // convert DataSet to XDocument
            XDocument sourceDocument = XDocument.Parse(data.GetXml());

            // complete XML according to schema
            IEnumerable<XElement> xassets = from xnode in sourceDocument.Descendants()
                                            where xnode.Name == _namespace + "Asset"
                                            && xnode.Descendants().Any()
                                            select xnode;

            foreach (XElement xasset in xassets)
            {
                string id = string.Empty;
                XElement idNode = (from node in xasset.Descendants()
                                   where node.Name.LocalName.ToUpper() == "ID"
                                   select new XElement(XName.Get("ID", _namespace.NamespaceName), node.Value))
                                  .SingleOrDefault();

                if (idNode != null)
                    id = idNode.Value;
                yield return GetAsset(xasset, id, at, bindings);
            }
        }

        /// <summary>
        /// Returns the XML element which represents asset
        /// </summary>
        /// <param name="dataSource">DataSource item which represents only one asset item.</param>
        /// <returns></returns>
        private XElement GetAsset(XElement dataSource, string id, AssetType at, BindingInfo bindings)
        {
            var attributes = new XElement(XName.Get("Attributes", _namespace.NamespaceName),
                GetAttributesNodes(dataSource, at, bindings).ToList());

            return id == string.Empty
                ? new XElement(XName.Get("Asset", _namespace.NamespaceName),
                    attributes)
                : new XElement(XName.Get("Asset", _namespace.NamespaceName),
                    new XElement(XName.Get("ID", _namespace.NamespaceName), id),
                    attributes);
        }

        /// <summary>
        /// Gets the attributes nodes for userXML element
        /// </summary>       
        private IEnumerable<XElement> GetAttributesNodes(XElement dataSource, AssetType at, BindingInfo bindings)
        {
            foreach (var attribute in at.Attributes)
            {
                var nodeValue = string.Empty;
                var binding = bindings.Bindings.SingleOrDefault(b => b.DestinationAttributeId == attribute.ID && attribute.ID > 0);
                if (binding != null)
                {
                    var xattribute = (from node in dataSource.Descendants()
                        where XmlConvert.DecodeName(node.Name.LocalName) == binding.DataSourceFieldName
                        select node).SingleOrDefault();
                    nodeValue = xattribute != null
                        ? xattribute.Value
                        : string.Empty;
                }
                yield return GetAttributeNode(
                    attribute, 
                    nodeValue, 
                    binding != null 
                        ? binding.DefaultValue 
                        : string.Empty,
                    binding != null && binding.DestinationRelatedAttributeId.HasValue 
                        ? binding.DestinationRelatedAttributeId.ToString()
                        : string.Empty);
            }
        }

        /// <summary>
        /// Returns the XML node which represents the asset's attribute
        /// </summary>
        private XElement GetAttributeNode(AssetTypeAttribute attribute, string nodeValue, string defaultValue, string relatedId)
        {
            return new XElement(XName.Get("Attribute", _namespace.NamespaceName),
                new XElement(XName.Get("Id", _namespace.NamespaceName), attribute.ID),
                new XElement(XName.Get("Name", _namespace.NamespaceName),  XmlConvert.DecodeName(attribute.Name)),
                GetValueSubNode(attribute, nodeValue, defaultValue),
                new XElement(XName.Get("FallbackValue", _namespace.NamespaceName), defaultValue),
                new XElement(XName.Get("RelatedAttributeId", _namespace.NamespaceName), relatedId));
        }
    }
}
