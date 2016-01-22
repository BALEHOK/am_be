using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Providers;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Classes.IE.Providers;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.Exceptions;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AppFramework.Core.Classes.IE.Adapters
{
    public interface IXMLToAssetsAdapter
    {
        StatusInfo Status { get; }

        /// <summary>
        /// Returns the Result of assets retrieving
        /// </summary>
        /// <returns></returns>
        List<Asset> GetEntities(string xmlpath, AssetType at);

        /// <summary>
        /// Retrieves real assets from XML
        /// </summary>
        /// <param name="document">XDocument</param>
        /// <param name="at"></param>
        /// <param name="bindings"></param>
        /// <returns>Collection of Assets</returns>
        IEnumerable<Asset> GetEntities(XDocument document, AssetType at);
    }

    /// <summary>
    /// Extracts assets from XML document
    /// </summary>
    public class XMLToAssetsAdapter : IXMLToAssetsAdapter
    {
        public StatusInfo Status { get; private set; }
        private readonly XNamespace _namespace = "http://tempuri.org/AssetManagementAssets.xsd";
        private readonly ILinkedEntityFinder _linkedEntityFinder;
        private readonly PasswordProvider _passwordProvider;
        private readonly IBarcodeProvider _barcodeProvider;
        private readonly ILog _logger;
        private readonly IAssetsService _assetsService;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public XMLToAssetsAdapter(
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository,
            ILinkedEntityFinder linkedEntityFinder,
            IBarcodeProvider barcodeProvider,
            ILog logger = null)
        {
            if (assetsService == null)
                throw new ArgumentNullException("IAssetsService");
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (linkedEntityFinder == null)
                throw new ArgumentNullException("ILinkedEntityFinder");
            if (barcodeProvider == null)
                throw new ArgumentNullException("IBarcodeProvider");
            Status = new StatusInfo();

            _assetsService = assetsService;
            _linkedEntityFinder = linkedEntityFinder;
            _passwordProvider = new PasswordProvider();
            _barcodeProvider = barcodeProvider;
            _logger = logger ?? LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Returns the Result of assets retrieving
        /// </summary>
        /// <returns></returns>
        public List<Asset> GetEntities(string xmlpath, AssetType at)
        {
            using (var provider = new XMLProvider(
                new ProviderParameters {{ProviderParameter.ReadPath, xmlpath}}))
            {
                var reader = provider.Read();
                return GetEntities(reader.Data, at).ToList();
            }
        }

        /// <summary>
        /// Retrieves real assets from XML
        /// </summary>
        /// <param name="document">XDocument</param>
        /// <param name="at"></param>
        /// <param name="bindings"></param>
        /// <returns>Collection of Assets</returns>
        public IEnumerable<Asset> GetEntities(XDocument document, AssetType at)
        {
            // xml assets
            var xassets = (from node in document.Descendants()
                where node.Name == _namespace + "Asset"
                select node).ToList();

            _logger.DebugFormat("Importing assets of type {0}...", at.NameInvariant);
            foreach (var xasset in xassets)
            {
                // set asset id
                long id = 0;
                var strId = (from xitem in xasset.Elements()
                                where xitem.Name == _namespace + "ID" || xitem.Name == _namespace + AttributeNames.DynEntityId
                                select xitem.Value).FirstOrDefault();
                long.TryParse(strId, out id);

                var asset = _assetsService.CreateAsset(at);
                asset[AttributeNames.DynEntityId].Value = id.ToString();

                // fill each attribute with value from xml
                _logger.DebugFormat("Importing asset no. {0}.",
                    xassets.IndexOf(xasset) + 1);
                try
                {
                    AssignAttributesValues(xasset, asset);
                }
                catch (ImportException ex)
                {
                    _logger.Error(ex);
                    continue;
                }

                yield return asset;
            }
        }

        private void AssignAttributesValues(XElement xasset, Asset asset)
        {
            foreach (var attribute in asset.Attributes)
            {
                // select attribute by default convention
                var xattribute = (from xat in xasset.Descendants()
                                  where xat.Name == _namespace + "Name" && xat.Value == attribute.GetConfiguration().Name
                                  select xat.Parent).SingleOrDefault();

                if (xattribute == null
                    && attribute.Configuration.IsRequired
                    && attribute.Configuration.Editable)
                {
                    if (attribute.Configuration.Name == AttributeNames.Barcode)
                        attribute.Value = _barcodeProvider.GenerateBarcode();
                    else if (attribute.Configuration.Name == AttributeNames.PermissionOnUsersEx)
                        attribute.Value = "0"; //TODO: Refactor 0 - no permissions for person only
                    else if (attribute.Configuration.Name == AttributeNames.Role)
                        attribute.Value = PredefinedRoles.OnlyPerson.ToString();
                    else if (attribute.Configuration.Name != AttributeNames.Password &&
                             attribute.Configuration.Name != AttributeNames.Email)
                        throw new ImportException(
                            string.Format("Required attribute missing: {0}",
                                attribute.Configuration.Name));
                }
                else if (xattribute != null)
                {
                    try
                    {
                        AssingValue(attribute, xattribute);
                    }
                    catch (FieldLinkingException ex)
                    {
                        _logger.Error(ex);
                        if (attribute.Configuration.IsRequired)
                            throw new ImportException(
                                string.Format("Required attribute missing: {0}",
                                    attribute.Configuration.Name));
                        else continue;
                    }
                }
                else
                {
                    _logger.DebugFormat("No value assigned to attribute '{0}'",
                        attribute.Configuration.Name);
                }
            }
        }

        private void AssingValue(AssetAttribute attribute, XElement xAssetAttribute)
        {
            XElement xnode = null;
            string nodeValue;

            switch (attribute.GetConfiguration().DataTypeEnum)
            {
                case Enumerators.DataType.DateTime:
                    nodeValue = (from attr in xAssetAttribute.Descendants()
                                 where attr.Name == _namespace + "Value"
                                 select attr.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(nodeValue))
                    {
                        double oaDate;
                        attribute.Value = double.TryParse(nodeValue, out oaDate)
                            ? DateTime.FromOADate(oaDate).ToString()
                            : nodeValue;
                        _logger.DebugFormat("Value '{0}' assigned to attribute '{1}'.",
                            attribute.Value, attribute.GetConfiguration().Name);
                    }
                    break;

                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Document:
                    nodeValue = (from attr in xAssetAttribute.Descendants()
                                 where attr.Name == _namespace + "Value"
                                 select attr.Value).SingleOrDefault();

                    var fallbackValue = (from attr in xAssetAttribute.Descendants()
                                     where attr.Name == _namespace + "FallbackValue"
                                     select attr.Value).SingleOrDefault();

                    if (!string.IsNullOrEmpty(nodeValue))
                    {
                        var relatedAttributeId = (from attr in xAssetAttribute.Descendants()
                                                  where attr.Name == _namespace + "RelatedAttributeId"
                                                  select attr.Value).SingleOrDefault();

                        if (string.IsNullOrEmpty(relatedAttributeId))
                            throw new ImportException("RelatedAttributeId attribute is missing: "
                                + xAssetAttribute);

                        var assetTypeId = attribute.GetConfiguration().RelatedAssetTypeID.Value;
                        var assetTypeAttributeId = long.Parse(relatedAttributeId);
                        var assetType = _assetTypeRepository.GetById(assetTypeId);

                        try
                        {
                            attribute.ValueAsId = _linkedEntityFinder.FindRelatedAssetId(
                                assetType, assetTypeAttributeId, nodeValue);
                        }
                        catch (FieldLinkingException ex)
                        {
                            if (!string.IsNullOrEmpty(fallbackValue))
                            {
                                _logger.DebugFormat("Fallback to '{0}' by reason: {1}", 
                                    fallbackValue, ex.Message);
                                attribute.ValueAsId = _linkedEntityFinder.FindRelatedAssetId(
                                    assetType, assetTypeAttributeId, fallbackValue);
                            }
                            else
                            {
                                throw;
                            }
                        }
                        _logger.DebugFormat("Value '{0}' assigned to attribute '{1}' by linking asset on value '{2}'.",
                                attribute.ValueAsId, attribute.GetConfiguration().Name, nodeValue);
                    }
                    break;

                case Enumerators.DataType.Assets:
                    // whole node "MultipleAssets"
                    xnode = (from attr in xAssetAttribute.Descendants()
                             where attr.Name == _namespace + "MultipleAssets"
                             select attr).SingleOrDefault();
                    var assets = GetMultipleAssets(attribute.GetConfiguration().RelatedAssetTypeID.Value, xnode);
                    assets.ForEach(a => attribute.MultipleAssets.Add(a));
                    break;

                case Enumerators.DataType.DynList:
                case Enumerators.DataType.DynLists:
                    // whole node "DynamicLists"
                    xnode = (from attr in xAssetAttribute.Descendants()
                             where attr.Name == _namespace + "DynamicLists"
                             select attr).SingleOrDefault();
                    attribute.DynamicListValues = GetDynamicListValues(xnode, attribute);
                    break;

                case Enumerators.DataType.Barcode:
                    string defaulBarcode = _barcodeProvider.DefaultValue;
                    // text of node "Value"
                    nodeValue = (from attr in xAssetAttribute.Descendants()
                                 where attr.Name == _namespace + "Value"
                                 select attr.Value).SingleOrDefault();

                    if (!string.IsNullOrEmpty(nodeValue) && nodeValue != defaulBarcode)
                        attribute.Value = nodeValue;
                    else
                        attribute.Value = _barcodeProvider.GenerateBarcode();
                    break;

                case Enumerators.DataType.Password:
                    nodeValue = (from attr in xAssetAttribute.Descendants()
                                 where attr.Name == _namespace + "Value"
                                 select attr.Value).SingleOrDefault();
                    if (!string.IsNullOrEmpty(nodeValue))
                    {
                        nodeValue = _passwordProvider.Encrypt(nodeValue);
                        attribute.Value = nodeValue;
                    }
                    break;

                default:
                    // text of node "Value"
                    nodeValue = (from attr in xAssetAttribute.Descendants()
                                 where attr.Name == _namespace + "Value"
                                 select attr.Value).SingleOrDefault();

                    if (!string.IsNullOrEmpty(nodeValue))
                    {
                        attribute.Value = nodeValue;
                        _logger.DebugFormat("Value '{0}' assigned to attribute '{1}'.",
                            attribute.Value, attribute.GetConfiguration().Name);
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns the list of related assets
        /// </summary>
        /// <param name="xnode"></param>
        /// <returns></returns>
        private List<KeyValuePair<long, string>> GetMultipleAssets(long atId, XElement xnode)
        {
            var result = new List<KeyValuePair<long, string>>();
            if (xnode != null)
            {
                IEnumerable<XElement> multipleAssets = from masset in xnode.Descendants()
                                                       where masset.Name == _namespace + "MultipleAsset"
                                                       select masset;

                foreach (XElement xmasset in multipleAssets)
                {
                    string strId = (from idnode in xmasset.Descendants()
                                    where idnode.Name == _namespace + "ID"
                                    select idnode.Value).SingleOrDefault();

                    string val = (from idnode in xmasset.Descendants()
                                  where idnode.Name == _namespace + "Value"
                                  select idnode.Value).SingleOrDefault();

                    long assetId;
                    if (!string.IsNullOrEmpty(val))
                    {
                        assetId = _linkedEntityFinder.FindAssetInIndex(atId, val);
                        if (assetId > 0)
                            result.Add(new KeyValuePair<long, string>(assetId, val ?? string.Empty));
                    }
                    else if (!string.IsNullOrEmpty(strId) && long.TryParse(strId, out assetId))
                    {
                        if (assetId > 0)
                            result.Add(new KeyValuePair<long, string>(assetId, val ?? string.Empty));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the list of assigned DynList values
        /// </summary>
        /// <param name="xnode"></param>
        /// <returns></returns>
        private List<DynamicListValue> GetDynamicListValues(XElement xnode, AssetAttribute attribute)
        {
            var result = new List<DynamicListValue>();
            if (xnode != null)
            {
                IEnumerable<XElement> dynLists = from dlists in xnode.Descendants()
                                                 where dlists.Name == _namespace + "DynList"
                                                 select dlists;

                foreach (XElement list in dynLists)
                {
                    long dynListId = 0;
                    string sDynListId = (from idnode in list.Descendants()
                                         where idnode.Name == _namespace + "DynListId"
                                         select idnode.Value).SingleOrDefault();
                    long.TryParse(sDynListId, out dynListId);

                    long parentListId = 0;
                    string strListId = (from idnode in list.Descendants()
                                        where idnode.Name == _namespace + "ParentListId"
                                        select idnode.Value).SingleOrDefault();
                    long.TryParse(strListId, out parentListId);

                    string value = (from valuenode in list.Descendants()
                                    where valuenode.Name == _namespace + "Value"
                                    select valuenode.Value).SingleOrDefault();

                    long dynListItemId = 0;
                    string sdynListItemId = (from idnode in list.Descendants()
                                             where idnode.Name == _namespace + "DynListItemId"
                                             select idnode.Value).SingleOrDefault();
                    long.TryParse(sdynListItemId, out dynListItemId);

                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        result.Add(new DynamicListValue()
                                       {
                                           DynamicListItemUid = dynListItemId,
                                           DynamicListUid = dynListId,
                                           ParentListId = parentListId,
                                           Value = value,
                                           AssetAttribute = attribute
                                       });
                    }
                }
            }
            return result;
        }
    }
}
