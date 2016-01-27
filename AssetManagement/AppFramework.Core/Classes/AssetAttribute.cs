using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DAL;
using AppFramework.Entities;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes
{
    [Serializable()]
    public class AssetAttribute
    {
        /// <summary>
        /// Gets and sets attribute value.
        /// </summary>
        [XmlElement(Order = 1)]
        public string Value
        {
            get
            {
                if (!_isInitialized)
                {
                    _initValue();
                    _isInitialized = true;
                }
                return _value;
            }
            set
            {
                if (Configuration.DataTypeEnum == Enumerators.DataType.Bool)
                {
                    bool outVal;
                    if (bool.TryParse(value, out outVal))
                    {
                        _value = outVal.ToString();
                    }
                    else
                    {
                        if (value == "1") _value = bool.TrueString;
                        else if (value == "0") _value = bool.FalseString;
                        else _value = bool.FalseString;
                    }
                }
                else
                {
                    _value = value;
                }
                _isInitialized = true;
                _data.Value = _value;
            }
        }

        private bool _isInitialized;

        /// <summary>
        /// Gets and sets the ID value of asset if this attribute is type of asset
        /// </summary>
        [XmlIgnore]
        public long? ValueAsId
        {
            get
            {
                if (_data.Value is long)
                {
                    return (long) _data.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _data.Value = value;
                _value = value.ToString();
                _isInitialized = false;
            }
        }

        /// <summary>
        /// Gets or sets the list of attribute names which should be displayed on a panel (for Screens functionality)
        /// </summary>
        [XmlIgnore]
        public List<string> CustomMultipleAssetsFields { get; set; }

        [XmlElement(Order = 0)]
        public AssetTypeAttribute Configuration
        {
            get { return this._configuration; }
            set { this._configuration = value; }
        }

        [XmlIgnore]
        public Asset RelatedAsset
        {
            get { return _relatedAsset ?? (_relatedAsset = _assetsService.GetRelatedAssetByAttribute(this)); }
            set { _relatedAsset = value; }
        }

        /// <summary>
        /// Gets if this attribute is Identity key in database
        /// </summary>
        [XmlIgnore()]
        public bool IsIdentity
        {
            get { return this.Configuration.Name == AttributeNames.DynEntityUid; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dynamic list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dynamic list; otherwise, <c>false</c>.
        /// </value>
        [XmlElement(Order = 2)]
        public bool IsDynamicList
        {
            get
            {
                return _configuration.DataTypeEnum == Enumerators.DataType.DynLists ||
                       _configuration.DataTypeEnum == Enumerators.DataType.DynList;
            }
        }

        /// <summary>
        /// Have context
        /// </summary>
        [XmlIgnore()]
        public bool IsWithContext
        {
            get { return _configuration.ContextId != null; }
        }

        /// <summary>
        /// Gets the list of assets if this attribute type of multiple assets. Pair: ID - display name
        /// </summary>
        [XmlIgnore]
        public List<KeyValuePair<long, string>> MultipleAssets
        {
            get
            {
                return _multipleAssets 
                    ?? (_multipleAssets = _assetsService
                    .GetRelatedAssetsByAttribute(this)
                    .Select(a => new KeyValuePair<long, string>(a.Id, a.Name))
                    .ToList());                
            }
        }

        /// <summary>
        /// Gets or sets the dynamic list values.
        /// </summary>
        /// <value>The dynamic list values.</value>
        [XmlElement(Order = 3)]
        public List<DynamicListValue> DynamicListValues
        {
            get
            {
                if (this._dynamicListValues == null && _configuration != null)
                {
                    if (_configuration.DataTypeEnum == Enumerators.DataType.DynList
                        || _configuration.DataTypeEnum == Enumerators.DataType.DynLists)
                    {
                        if (ParentAsset != null && ParentAsset.UID > 0)
                        {
                            _dynamicListValues = _dynamicListsService
                                .GetLegacyListValues(Configuration, ParentAsset.UID)
                                .ToList();
                            _dynamicListValues.ForEach(lv => lv.AssetAttribute = this);
                        }
                        else
                        {
                            _dynamicListValues = new List<DynamicListValue>();
                        }
                    }
                }
                return _dynamicListValues;
            }
            set { _dynamicListValues = value; }
        }

        [XmlIgnore]
        public Asset ParentAsset { get; set; }

        [XmlIgnore]
        public List<ValidationResult> ValidationResults { get; set; }

        [XmlIgnore]
        public DynColumn Data { get { return _data; } }  

        private string _value = string.Empty;
        private List<KeyValuePair<long, string>> _multipleAssets;
        private List<DynamicListValue> _dynamicListValues;
        private AssetTypeAttribute _configuration;
        private DynColumn _data;
        private Asset _relatedAsset;

        private readonly IAttributeValueFormatter _attributeValueFormatter;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDynamicListsService _dynamicListsService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AssetAttribute()
        {
            _data = new DynColumn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetAttribute"/> class.
        /// </summary>
        /// <param name="attrConfig">The attr config.</param>
        /// <param name="data">The data.</param>
        /// <param name="parent">Parent asset</param>
        public AssetAttribute(
            AssetTypeAttribute attrConfig, 
            DynColumn data, 
            Asset parent,
            IAttributeValueFormatter attributeValueFormatter, 
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            IUnitOfWork unitOfWork,
            IDynamicListsService dynamicListsService)
            : this(attrConfig, data, attributeValueFormatter,
                assetTypeRepository, assetsService, unitOfWork, dynamicListsService)
        {
            ParentAsset = parent;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="attrConfig">AssetTypeAttribute data</param>
        /// <param name="data">Asset data</param>
        private AssetAttribute(
            AssetTypeAttribute attrConfig, 
            DynColumn data,
            IAttributeValueFormatter attributeValueFormatter,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService,
            IUnitOfWork unitOfWork,
            IDynamicListsService dynamicListsService)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (attributeValueFormatter == null)
                throw new ArgumentNullException("attributeValueFormatter");
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (dynamicListsService == null)
                throw new ArgumentNullException("dynamicListsService");
            _dynamicListsService = dynamicListsService;
            _attributeValueFormatter = attributeValueFormatter;
            _configuration = attrConfig;
            _data = data;
        }

        private void _initValue()
        {
            if (_data != null &&
                _data.Value != null &&
                !string.IsNullOrEmpty(_data.Value.ToString()))
            {
                switch (_configuration.DataTypeEnum)
                {
                    case Enumerators.DataType.Asset:
                    case Enumerators.DataType.Document:
                    case Enumerators.DataType.Role:
                    case Enumerators.DataType.DynList:
                    case Enumerators.DataType.DynLists:
                        _value = _attributeValueFormatter.GetDisplayValue(
                            Configuration,
                            Data.Value,
                            IsActive());
                        break;

                    case Enumerators.DataType.Assets:
                        _value = string.Join(", ", from pair in this.MultipleAssets
                                                   select pair.Value.Trim());
                        break;

                    case Enumerators.DataType.Bool:
                        _value = _data.Value.ToString();
                        break;

                    case Enumerators.DataType.Float:
                        _value = string.Format(ApplicationSettings.DisplayCultureInfo, "{0}",
                                               float.Parse(_data.Value.ToString(), NumberStyles.Float,
                                                           ApplicationSettings.PersistenceCultureInfo));
                        break;

                    case Enumerators.DataType.Money:
                    case Enumerators.DataType.USD:
                    case Enumerators.DataType.Euro:
                        _value = string.Format(ApplicationSettings.DisplayCultureInfo, "{0}",
                                               float.Parse(_data.Value.ToString(), NumberStyles.Currency,
                                                           ApplicationSettings.PersistenceCultureInfo));
                        break;

                    default:
                        if (_configuration.DataType.FrameworkDataType != null)
                        {
                            _value = string.Format(ApplicationSettings.DisplayCultureInfo, "{0}",
                                                   Convert.ChangeType(_data.Value,
                                                                      _configuration.DataType.FrameworkDataType,
                                                                      ApplicationSettings.PersistenceCultureInfo));
                        }
                        else
                        {
                            _value = _data.Value.ToString();
                        }
                        break;
                }
            }
        }               

        private bool IsActive()
        {
            var version = ParentAsset[AttributeNames.ActiveVersion].Value;
            bool isActive;
            if (string.IsNullOrWhiteSpace(version))
            {
                isActive = true;
            }
            else if (version.Length > 1)
            {
                isActive = Convert.ToBoolean(version);
            }
            else
            {
                isActive = int.Parse(version) > 0;
            }
            return isActive;
        }

        /// <summary>
        /// Returns the configuration of this attribute
        /// </summary>
        /// <returns></returns>
        public AssetTypeAttribute GetConfiguration()
        {
            return this._configuration;
        }

        public DynColumn GetDataEntity()
        {
            return this._data;
        }

        /// <summary>
        /// Adds the dynamic list value.
        /// </summary>
        /// <param name="dlv">The DLV.</param>
        [Obsolete]
        public void AddDynamicListValue(DynamicListValue dlv)
        {
            if (_dynamicListValues == null)
                _dynamicListValues = new List<DynamicListValue>();
            dlv.AssetAttribute = this;
            _dynamicListValues.Add(dlv);
        }
        
        /// <summary>
        /// Inits the internal data (associated datarow). This method can be used in case when attribute was created not from database data, but for XML import.
        /// To restore missed _data field after import, use InitData
        /// </summary>
        public void InitData()
        {
            var adapter = new DynColumnAdapter(new DataTypeService(_unitOfWork));
            _data = adapter.ConvertDynEntityAttribConfigToDynColumn(Configuration.Base);
        }

        public override string ToString()
        {
            return string.Format("#{1}, {0} ({2})", Value, ValueAsId, GetConfiguration().Name);
        }

        public DateTime GetValueAsDateTime()
        {
            if (_data.Value == null || string.IsNullOrEmpty(_data.Value.ToString()) ||
                _configuration.DataTypeEnum != Enumerators.DataType.DateTime)
                throw new InvalidCastException();
            return
                (DateTime)
                Convert.ChangeType(_data.Value, _configuration.DataType.FrameworkDataType,
                                   ApplicationSettings.PersistenceCultureInfo);
        }
    }
}
