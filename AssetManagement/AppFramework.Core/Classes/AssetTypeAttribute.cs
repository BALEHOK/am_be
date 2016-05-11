
using AppFramework.Core.Classes.Extensions;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes.DynLists;
    using AppFramework.Core.Classes.Validation;
    using AppFramework.Core.Classes.Validation.Expression;
    using AppFramework.Core.ConstantsEnumerators;
    using AppFramework.Core.Helpers;
    using AppFramework.Core.Interfaces;
    using AppFramework.Core.Properties;
    using AppFramework.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(CustomDataType))]
    [XmlInclude(typeof(AssetDataType))]
    [XmlInclude(typeof(TypeOfAssetAttribute))]
    public class AssetTypeAttribute : IRevision, ISerializable
    {
        /// <summary>
        /// Is description for asset
        /// </summary> 
        public bool IsDescription
        {
            get { return _base.IsDescription; }
            set { _base.IsDescription = value; }
        }

        /// <summary>
        /// Gets and sets if this attribute is identity
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return _base.Name == AttributeNames.DynEntityUid;
            }
        }

        /// <summary>
        /// Gets and sets if this attribute is one of predefined app types
        /// </summary>
        public PredefinedEntity? RelatedEntity { get; set; }

        /// <summary>
        /// Gets and sets attribute name
        /// </summary>
        [XmlIgnore]
        public string NameLocalized
        {
            get
            {
                return _base.Name.Localized();
            }
        }

        public string Name
        {
            get
            {
                return _base.Name;
            }
            set
            {
                _base.Name = value;
                if (string.IsNullOrEmpty(DBTableFieldName))
                    DBTableFieldName = value;
                if (string.IsNullOrEmpty(_base.Label))
                    _base.Label = value;
            }
        }

        /// <summary>
        /// Gets and sets attribute description
        /// </summary>
        public string Comment
        {
            get { return _base.Comment; }
            set { _base.Comment = value; }
        }

        /// <summary>
        /// Gets the value of current datatype by enum
        /// </summary>
        [XmlIgnore]
        public Enumerators.DataType DataTypeEnum
        {
            get { return DataType.Code; }
        }

        /// <summary>
        /// Returns the DataType object
        /// </summary>
        /// <returns></returns>
        public virtual DataTypeBase DataType
        {
            get
            {
                if (_customDataType != null) 
                    return _customDataType;
                if (_base.DataType != null)
                    _customDataType = new CustomDataType(_base.DataType);
                else if (_base.DataTypeUid > 0)
                    _customDataType = _dataTypeService.GetByUid(_base.DataTypeUid);
                return _customDataType;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _customDataType = value;
                _base.DataType = _customDataType.Base;
            }
        }

        /// <summary>
        /// This property needed for (de)serialization of data type as a string
        /// </summary>
        public string DataTypeName
        {
            get { return DataType.Name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DataType = _dataTypeService.GetByName(value);
                }
            }
        }

        /// <summary>
        /// Gets and sets attribute label
        /// </summary>
        public string Label
        {
            get { return _base.Label; }
            set { _base.Label = value; }
        }

        /// <summary>
        /// Gets and sets if it's dyn list item
        /// </summary>
        public bool IsDynListValue
        {
            get { return _base.IsDynListValue; }
            set { _base.IsDynListValue = value; }
        }

        /// <summary>
        /// Gets and sets if it's financial information
        /// </summary>
        public bool IsFinancialInfo
        {
            get { return _base.IsFinancialInfo; }
            set { _base.IsFinancialInfo = value; }
        }

        /// <summary>
        /// Gets and sets if this attribute is required
        /// </summary>
        public bool IsRequired
        {
            get { return _base.IsRequired; }
            set { _base.IsRequired = value; }
        }

        /// <summary>
        /// Gets and sets if this attribute is keyword while searching an asset
        /// </summary>
        public bool IsKeyword
        {
            get { return _base.IsKeyword; }
            set { _base.IsKeyword = value; }
        }

        /// <summary>
        /// Gets and sets if this attribute should be included in index
        /// </summary>
        public bool IsFullIndex
        {
            get { return _base.IsFullTextInidex; }
            set { _base.IsFullTextInidex = value; }
        }

        public bool IsCalculated
        {
            get { return !string.IsNullOrWhiteSpace(_base.CalculationFormula); }
        }

        public string FormulaText
        {
            get { return _base.CalculationFormula != null ? _base.CalculationFormula.Trim() : null; }
            set
            {
                var text = value != null ? value.Trim() : null;
                _base.CalculationFormula = text;
            }
        }

        /// <summary>
        /// Gets and sets if this value should be shown in searching result teaser
        /// </summary>
        public bool DisplayOnResultList
        {
            get { return _base.DisplayOnResultList; }
            set { _base.DisplayOnResultList = value; }
        }

        /// <summary>
        /// Gets and sets display order in searching result list
        /// </summary>
        public int DisplayOrderResultList
        {
            get
            {
                if (_base.DisplayOrderResultList != null)
                {
                    return (int)_base.DisplayOrderResultList;
                }
                else
                {
                    return default(int);
                }
            }
            set { _base.DisplayOrderResultList = value; }
        }

        /// <summary>
        /// Gets and sets if this attribute should be shown in extended search 
        /// </summary>
        public bool DisplayOnExtResultList
        {
            get
            {
                if (_base.DisplayOnExtResultList != null)
                {
                    return (bool)_base.DisplayOnExtResultList;
                }
                else
                {
                    return false;
                }
            }
            set { _base.DisplayOnExtResultList = value; }
        }

        /// <summary>
        /// Gets and sets order of this attribute in extended search result
        /// </summary>
        public int DisplayOrderExtResultList
        {
            get
            {
                if (_base.DisplayOrderExtResultList != null)
                {
                    return (int)_base.DisplayOrderExtResultList;
                }
                return default(int);
            }
            set { _base.DisplayOrderExtResultList = value; }
        }

        /// <summary>
        /// Gets the revision number
        /// </summary>
        public int Revision
        {
            get { return _base.Revision; }
            set { _base.Revision = value; }
        }

        /// <summary>
        /// Gets if this version is active version
        /// </summary>
        public bool IsActiveVersion
        {
            get { return _base.ActiveVersion; }
            set { _base.ActiveVersion = value; }
        }

        /// <summary>
        /// Gets the ID of attribute
        /// </summary>
        public long ID
        {
            get { return _base.DynEntityAttribConfigId; }
            set { _base.DynEntityAttribConfigId = value; }
        }

        /// <summary>
        /// Gets the unique ID of attribute
        /// </summary>
        public long UID
        {
            get
            {
                if (_identifier == 0)
                {
                    if (_base.DynEntityAttribConfigUid > 0)
                    {
                        _identifier = _base.DynEntityAttribConfigUid;
                    }
                    else
                    {
                        _identifier = Randomization.GetIdentifier();
                    }
                }
                return _identifier;
            }
            set { _identifier = value; }
        }

        /// <summary>
        /// Gets and sets if this attribute is active
        /// </summary>
        public bool IsActive
        {
            get { return _base.Active; }
            set { _base.Active = value; }
        }

        /// <summary>
        /// Gets and sets the context of attribute
        /// </summary>
        public long? ContextId
        {
            get { return _base.ContextId; }
            set { _base.ContextId = value > 0 ? value : null; }
        }

        /// <summary>
        /// Gets and sets the parent entity, which holds this attribute
        /// </summary>
        public long AssetTypeUID
        {
            get { return _base.DynEntityConfigUid; }
            set { _base.DynEntityConfigUid = value; }
        }

        /// <summary>
        /// Gets and sets the DB field name of this attribute
        /// </summary>
        public string DBTableFieldName
        {
            get
            {
                return _base.DBTableFieldname;
            }
            set
            {
                _base.DBTableFieldname = Routines.SanitizeDBObjectName(value);
            }
        }

        /// <summary>
        /// Gets if this attribute is type of asset or not
        /// </summary>
        [XmlIgnore]
        public bool IsAsset
        {
            get
            {
                return (this.DataTypeEnum == Enumerators.DataType.Asset
                    || this.DataTypeEnum == Enumerators.DataType.Document);
            }
        }

        /// <summary>
        /// Gets if this attribute is list of assets or not
        /// </summary>
        [XmlIgnore]
        public bool IsMultipleAssets
        {
            get
            {
                return this.DataTypeEnum == Enumerators.DataType.Assets;
            }
        }

        /// <summary>
        /// If this attribute is asset, gets and sets related AssetType ID
        /// </summary>
        public long? RelatedAssetTypeID
        {
            get { return _base.RelatedAssetTypeID; }
            set { _base.RelatedAssetTypeID = value; }
        }

        /// <summary>
        /// If this attribute is asset, 
        /// gets and sets related AssetTypeAttribute ID as display field of that asset 
        /// </summary>
        public long? RelatedAssetTypeAttributeID
        {
            get { return _base.RelatedAssetTypeAttributeID; }
            set { _base.RelatedAssetTypeAttributeID = value; }
        }

        public string ValidationExpr
        {
            get { return _base.ValidationExpr; }
            set { _base.ValidationExpr = value; }
        }

        public string ValidationMessage
        {
            get { return _base.ValidationMessage; }
            set { _base.ValidationMessage = value; }
        }

        /// <summary>
        /// Gets and sets if edition of configuration is allowed for user.
        /// </summary>
        public bool AllowEditConfig
        {
            get { return _base.AllowEditConfig; }
            set { _base.AllowEditConfig = value; }
        }

        /// <summary>
        /// Gets and sets if editing of value of this attribute is allowed for user.
        /// </summary>
        public bool AllowEditValue
        {
            get { return _base.AllowEditValue; }
            set { _base.AllowEditValue = value; }
        }

        /// <summary>
        /// Gets if this attribute can be modified by user or not
        /// </summary>
        [XmlIgnore]
        public bool Editable
        {
            get
            {
                return DataType.Editable && this.AllowEditValue;
            }
        }

        [XmlIgnore]
        public DynamicList DynamicList
        {
            get
            {
                if (_base.DynListUid.HasValue && _dynamicList == null)
                    _dynamicList = _dynListsService.GetByUid(_base.DynListUid.Value);
                return _dynamicList;
            }
        }
        private DynamicList _dynamicList;

        public long? DynamicListUid
        {
            get { return _base.DynListUid; }
            set { _base.DynListUid = value; _dynamicList = null; }
        }

        /// <summary>
        /// Gets and sets if this attribute should be shown in grid of assets
        /// </summary>
        public bool IsShownInGrid
        {
            get { return _base.IsShownInGrid; }
            set { _base.IsShownInGrid = value; }
        }

        /// <summary>
        /// Gets and sets if this attribute should be shown on asset panel
        /// </summary>
        public bool IsShownOnPanel
        {
            get { return _base.IsShownOnPanel; }
            set { _base.IsShownOnPanel = value; }
        }

        /// <summary>
        /// Gets the base DAL entity
        /// </summary>
        [XmlIgnore]
        public DynEntityAttribConfig Base
        {
            get { return _base; }
        }

        /// <summary>
        /// Gets and Sets the reference to parent entity,
        /// which holds this attribute
        /// </summary>
        /// <param name="parent"></param>
        [XmlIgnore]
        public AssetType Parent
        {
            get { return _parent; }
            private set { _parent = value; }
        }

        /// <summary>
        /// Ordering
        /// </summary>
        public int DisplayOrder
        {
            get { return _base.DisplayOrder; }
            set { _base.DisplayOrder = value; }
        }

        /// <summary>
        /// Gets if this attribute represents the UpdateUser field
        /// </summary>
        public bool IsUpdateUser
        {
            get
            {
                return (DataTypeEnum == Enumerators.DataType.Asset
                        && Name == "Update User");
            }
        }

        public bool IsUsedForNames
        {
            get { return _base.IsUsedForNames; }
            set { _base.IsUsedForNames = value; }
        }

        public int? NameGenOrder
        {
            get { return _base.NameGenOrder; }
            set { _base.NameGenOrder = value; }
        }

        public DateTime UpdateDate
        {
            get
            {
                return _base.UpdateDate;
            }
        }

        private readonly DynEntityAttribConfig _base;
        private List<AttributeValidationRule> _validationRules;
        private AssetType _parent;
        private long _identifier;
        private AssetType _assetType;
        private DataTypeBase _customDataType;
        private readonly IDataTypeService _dataTypeService;
        private readonly IDynamicListsService _dynListsService;

        /// <summary>
        /// Class constructor
        /// </summary>
        public AssetTypeAttribute()
            : this(
            new DynEntityAttribConfig(), 
            new UnitOfWork())
        {
            _base.ActiveVersion = true;
            _base.Active = true;
            _base.IsShownOnPanel = true;
            // allow edit config for new
            _base.AllowEditConfig = true;
            // allow edit value for new
            _base.AllowEditValue = true;

            // TODO: remove that lame stuff below
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! NEVER Miss this string!!!!
            _base.UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Class constructor with properties initialization from database info
        /// </summary>
        /// <param name="data">Record from DynEntityAttribConfig table</param>
        public AssetTypeAttribute(
            DynEntityAttribConfig data, 
            IUnitOfWork unitOfWork,
            AssetType parent = null)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _base = data;
            _base.StartTracking();
            _dataTypeService = new DataTypeService(unitOfWork);
            _dynListsService = new DynamicListsService(unitOfWork, new AttributeRepository(unitOfWork));
            Parent = parent;
        }
        
        public override string ToString()
        {
            return this.Name + " " + this.UID + " " + this.ID;
        }
    }
}
