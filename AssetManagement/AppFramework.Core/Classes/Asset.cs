using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DAL;
using AppFramework.Core.Interfaces;
using AppFramework.DataProxy;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class Asset : IRevision, IModification
    {
        /// <summary>
        /// Gets the unique ID of asset
        /// </summary>
        /// <value>The UID</value>
        [XmlElement]
        public long UID
        {
            get
            {
                var key = this[AttributeNames.DynEntityUid].Value;
                return string.IsNullOrEmpty(key) 
                    ? default(long)
                    : long.Parse(key);
            }
            set
            {
                this[AttributeNames.DynEntityUid].Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets the ID of asset
        /// </summary>
        [XmlIgnore]
        public long ID
        {
            get
            {
                return string.IsNullOrEmpty(this[AttributeNames.DynEntityId].Value) 
                    ? default(long) 
                    : long.Parse(this[AttributeNames.DynEntityId].Value);
            }
            set
            {
                this[AttributeNames.DynEntityId].Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlElement]
        public string Name
        {
            get
            {
                var attr = this[AttributeNames.Name];
                if (attr != null)
                    return attr.Value;
                else
                    return string.Empty;
            }
            set
            {
                var attr = this[AttributeNames.Name];
                if (attr != null)
                    attr.Value = value;
            }
        }

        [XmlIgnore]
        public string NavigateUrl
        {
            get
            {
                return string.Format("/Asset/View.aspx?AssetID={0}&AssetTypeID={1}", this.ID, this.GetConfiguration().ID);
            }
        }

        /// <summary>
        /// Gets the list of asset attributes
        /// </summary>
        [XmlArray]
        [XmlArrayItem("AssetAttribute", typeof(AssetAttribute))]
        public List<AssetAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _initAttributes();
                }
                return _attributes;
            }
            set
            {
                _attributes = value;
            }
        }

        /// <summary>
        /// Gets the public attributes - which not marked as IsInternal and can be displayed for user.
        /// </summary>
        /// <value>The attributes list.</value>
        [XmlIgnore]
        public List<AssetAttribute> AttributesPublic
        {
            get
            {
                return this.Attributes.Where(a => a.GetConfiguration().IsShownOnPanel).ToList();
            }
        }


        /// <summary>
        /// Gets the <see cref="AppFramework.Core.Classes.AssetAttribute"/> with the specified data table field name.
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        public AssetAttribute this[string name]
        {
            get
            {
                return Attributes.SingleOrDefault(
                    a => a.Configuration.DBTableFieldName.ToLower() == name.ToLower());
            }
        }

        public long DynEntityConfigUid { get { return long.Parse(this[AttributeNames.DynEntityConfigUid].Value); } }
        

        [XmlIgnore]
        public string Barcode
        {
            get 
            { 
                return this[AttributeNames.Barcode] != null 
                    ? this[AttributeNames.Barcode].Value 
                    : string.Empty; 
            }
        }

        /// <summary>
        /// Gets if this assets was not saved yet or not
        /// </summary>
        [XmlIgnore]
        public bool IsNew
        {
            get
            {
                long assetUID = 0;
                long.TryParse(this.Attributes.Single(g => g.IsIdentity).Value, out assetUID);
                long assetID = 0;
                long.TryParse(this.Attributes.Single(g => g.GetConfiguration().Name == AttributeNames.DynEntityId).Value, out assetID);
                return assetUID == 0 && assetID == 0;
            }
        }

        [XmlIgnore]
        public bool IsHistory
        {
            get
            {
                var version = this[AttributeNames.ActiveVersion].Value;
                if (version.Length > 1)
                {
                    return !Convert.ToBoolean(this[AttributeNames.ActiveVersion].Value);
                }
                else
                {
                    return !this.IsNew && int.Parse(this[AttributeNames.ActiveVersion].Value) == 0;
                }
            }
        }

        [XmlIgnore]
        public int Revision
        {
            get
            {
                var revision = this[AttributeNames.Revision].Value;
                return int.Parse(revision);
            }
            set
            {
                this[AttributeNames.Revision].Value = value.ToString();
            }
        }

        [XmlIgnore]
        public DateTime UpdatedAt
        {
            get
            {
                return ID == 0
                    ? DateTime.Now
                    : Convert.ToDateTime(
                        this[AttributeNames.UpdateDate].Value,
                        ApplicationSettings.DisplayCultureInfo.DateTimeFormat);
            }
        }

        [XmlIgnore]
        public AssetType Configuration
        {
            get { return this._configuration; }
            set { this._configuration = value; }
        }

        [XmlIgnore]
        public DynRow DataRow
        {
            get { return _data; }
        }

        private List<AssetAttribute> _attributes;
        private AssetType _configuration;
        private DynRow _data;

        /// <summary>
        /// Constructor for deserialization support
        /// </summary>
        public Asset()
        {
        }

        /// <summary>
        /// Asset constructor
        /// </summary>
        /// <param name="config">AssetType configuration</param>
        /// <param name="data">Datarow from asset table</param>
        public Asset(AssetType config, 
            DynRow data,
            IAttributeValueFormatter attributeValueFormatter,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            IUnitOfWork unitOfWork,
            IDynamicListsService dynamicListsService)
        {
            if (config == null)
                throw new ArgumentNullException("AssetType cannot be null");
            if (data == null)
                throw new ArgumentNullException("DynRow cannot be null");
            if (attributeValueFormatter == null)
                throw new ArgumentNullException("IAttributeValueFormatter");
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (dynamicListsService == null)
                throw new ArgumentNullException("dynamicListsService");
            _dynamicListsService = dynamicListsService;
            _configuration = config;
            _data = data;
            _attributeValueFormatter = attributeValueFormatter;
        }

        /// <summary>
        /// All attributes initialization
        /// </summary>
        private void _initAttributes()
        {
            _attributes = new List<AssetAttribute>(_data.Fields.Count);
            var attrConfigs = _configuration.AllAttributes;
            foreach (var column in _data.Fields)
            {
                var attrConfig = attrConfigs.SingleOrDefault(a => a.DBTableFieldName == column.Name);
                if (attrConfig != null)
                    _attributes.Add(new AssetAttribute(attrConfig, column, this, 
                        _attributeValueFormatter, _assetTypeRepository, _assetsService, _unitOfWork, _dynamicListsService));
            }
        }

        /// <summary>
        /// Returns the configuration of this asset
        /// </summary>
        /// <returns>AssetType object</returns>
        public AssetType GetConfiguration()
        {
            return this._configuration;
        }
        
        /// <summary>
        /// Creates the stock transaction.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="code">The code.</param>
        /// <param name="count">The count.</param>
        /// <param name="price">The price.</param>
        /// <param name="departmentId">The department id.</param>
        /// <param name="assetId">The asset id.</param>
        public void CreateStockTransaction(string title, Stock.TransactionTypeCode code, float count, float price, long departmentId, long assetId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the stock transaction.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="code">The code.</param>
        /// <param name="count">The count.</param>
        /// <param name="price">The price.</param>
        public void CreateStockTransaction(string title, Stock.TransactionTypeCode code, float count, float price)
        {
            this.CreateStockTransaction(title, code, count, price, 0, 0);
        }

        /// <summary>
        /// Serializes attributes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Serialize(System.IO.Stream stream)
        {
            // serialize only attributes
            var xmlSerializer = new XmlSerializer(typeof(List<AssetAttribute>));
            xmlSerializer.Serialize(stream, Attributes);
        }

        public void Deserialize(System.IO.Stream stream)
        {
            var asset = this;
            var xmlSerializer = new XmlSerializer(asset.Attributes.GetType());
            asset.Attributes = xmlSerializer.Deserialize(stream) as List<AssetAttribute>;
            foreach (var attribute in asset.Attributes)
            {
                attribute.ParentAsset = asset;
                attribute.InitData();
            }
        }

        public void MoveToNextLocation()
        {
            int locationIndex = -1, nextLocationIndex = -1;
            for (int i = 0; i < this.Attributes.Count; i++)
            {
                if (this.Attributes[i].GetConfiguration().DBTableFieldName == AttributeNames.LocationId)
                    locationIndex = i;

                if (this.Attributes[i].GetConfiguration().DBTableFieldName == AttributeNames.NextLocationId)
                    nextLocationIndex = i;
            }


            if (nextLocationIndex > 0 && nextLocationIndex > 0)
            {
                if (this.Attributes[nextLocationIndex].ValueAsId.HasValue)
                {
                    this.Attributes[locationIndex].Value = this.Attributes[nextLocationIndex].ValueAsId.ToString();
                    this.Attributes[locationIndex].ValueAsId = this.Attributes[nextLocationIndex].ValueAsId;
                    this.Attributes[nextLocationIndex].ValueAsId = 0;
                }
            }
        }

        /// <summary>
        /// Overrides default comparer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() == typeof(System.DBNull))
            {
                return false;
            }
            else
            {
                Asset compareAsset = (Asset)obj;
                return this.UID == compareAsset.UID &&
                    this.GetConfiguration().UID == compareAsset.GetConfiguration().UID;
            }
        }

        /// <summary>
        /// Returns a cache key for storing in session
        /// </summary>
        /// <returns></returns>
        public string GetCacheKey()
        {
            return string.Format("{0}_{1}", this.GetConfiguration().UID, this.UID);
        }

        public bool IsDeleted
        {
            get
            {
                return bool.Parse(_data[AttributeNames.IsDeleted].Value.ToString());
            }
            set
            {
                _data[AttributeNames.IsDeleted].Value = value.ToString();
            }
        }

        private readonly IAttributeValueFormatter _attributeValueFormatter;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDynamicListsService _dynamicListsService;
        
        /// <summary>
        /// Generates asset's name
        /// </summary>
        /// <returns>autogenerated name</returns>
        public string GenerateName()
        {
            var sortAttribs = Attributes.Where(a => a.Configuration.IsUsedForNames);
            var noOrder = sortAttribs.Where(a => !a.Configuration.NameGenOrder.HasValue);
            var ordered = sortAttribs.Where(a => a.Configuration.NameGenOrder.HasValue).OrderBy(a => a.Configuration.NameGenOrder.Value);
            var finalOrder = ordered.Union(noOrder);

            string tempValue = string.Join(", ", from value in
                                                     from a in finalOrder
                                                     select a.Value
                                                 where !string.IsNullOrEmpty(value)
                                                 select value);
            if (tempValue.Length > 255)
            {
                tempValue = tempValue.Substring(0, 251);
                tempValue += "...";
            }
            else if (string.IsNullOrEmpty(tempValue) || string.IsNullOrWhiteSpace(tempValue))
            {
                tempValue = Properties.Resources.AutoNameText;
            }
            Name = tempValue;
            return Name;
        }

        /// <summary>
        /// Returns the list of documents for current asset
        /// </summary>
        /// <returns></returns>
        public List<Asset> GetDocuments()
        {
            var documents = new List<Asset>();
            var documentTypeId = PredefinedAttribute.Get(PredefinedEntity.Document).DynEntityConfigID;
            var documentAssetType = _assetTypeRepository.GetById(documentTypeId);

            foreach (var attribute in AttributesPublic)
            {
                var attributeDataType = attribute.GetConfiguration().DataType.Code;
                if (attributeDataType == Enumerators.DataType.Document && attribute.ValueAsId > 0)
                {
                    var doc = _assetsService.GetAssetById(attribute.ValueAsId.Value, documentAssetType);
                    if (doc != null)
                        documents.Add(doc);
                }

                if (attribute.GetConfiguration().RelatedAssetTypeID == documentTypeId)
                {
                    if (attributeDataType == Enumerators.DataType.Asset && attribute.ValueAsId > 0)
                    {
                        var doc = _assetsService.GetAssetById(attribute.ValueAsId.Value, documentAssetType);
                        if (doc != null)
                            documents.Add(doc);
                    }

                    if (attributeDataType == Enumerators.DataType.Assets)
                    {
                        documents.AddRange(
                            attribute.MultipleAssets.Select(a => 
                                _assetsService.GetAssetById(a.Key, documentAssetType))
                                    .Where(doc => doc != null));
                    }
                }
            }
            return documents;
        }

        public override string ToString()
        {
            return Name;
        }   
    }
}

