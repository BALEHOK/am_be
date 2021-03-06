//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace AppFramework.Entities
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(AttributePanelAttribute))]
    [KnownType(typeof(Context))]
    [KnownType(typeof(DataType))]
    [KnownType(typeof(DynEntityConfig))]
    [KnownType(typeof(DynListValue))]
    [KnownType(typeof(DynList))]
    public partial class DynEntityAttribConfig: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long DynEntityAttribConfigUid
        {
            get { return _dynEntityAttribConfigUid; }
            set
            {
                if (_dynEntityAttribConfigUid != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'DynEntityAttribConfigUid' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _dynEntityAttribConfigUid = value;
                    OnPropertyChanged("DynEntityAttribConfigUid");
                }
            }
        }
        private long _dynEntityAttribConfigUid;
    
        [DataMember]
        public long DynEntityConfigUid
        {
            get { return _dynEntityConfigUid; }
            set
            {
                if (_dynEntityConfigUid != value)
                {
                    ChangeTracker.RecordOriginalValue("DynEntityConfigUid", _dynEntityConfigUid);
                    if (!IsDeserializing)
                    {
                        if (DynEntityConfig != null && DynEntityConfig.DynEntityConfigUid != value)
                        {
                            DynEntityConfig = null;
                        }
                    }
                    _dynEntityConfigUid = value;
                    OnPropertyChanged("DynEntityConfigUid");
                }
            }
        }
        private long _dynEntityConfigUid;
    
        [DataMember]
        public long DynEntityAttribConfigId
        {
            get { return _dynEntityAttribConfigId; }
            set
            {
                if (_dynEntityAttribConfigId != value)
                {
                    _dynEntityAttribConfigId = value;
                    OnPropertyChanged("DynEntityAttribConfigId");
                }
            }
        }
        private long _dynEntityAttribConfigId;
    
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        private string _name;
    
        [DataMember]
        public string NameTranslationId
        {
            get { return _nameTranslationId; }
            set
            {
                if (_nameTranslationId != value)
                {
                    _nameTranslationId = value;
                    OnPropertyChanged("NameTranslationId");
                }
            }
        }
        private string _nameTranslationId;
    
        [DataMember]
        public long DataTypeUid
        {
            get { return _dataTypeUid; }
            set
            {
                if (_dataTypeUid != value)
                {
                    ChangeTracker.RecordOriginalValue("DataTypeUid", _dataTypeUid);
                    if (!IsDeserializing)
                    {
                        if (DataType != null && DataType.DataTypeUid != value)
                        {
                            DataType = null;
                        }
                    }
                    _dataTypeUid = value;
                    OnPropertyChanged("DataTypeUid");
                }
            }
        }
        private long _dataTypeUid;
    
        [DataMember]
        public Nullable<long> DynListUid
        {
            get { return _dynListUid; }
            set
            {
                if (_dynListUid != value)
                {
                    ChangeTracker.RecordOriginalValue("DynListUid", _dynListUid);
                    if (!IsDeserializing)
                    {
                        if (DynList != null && DynList.DynListUid != value)
                        {
                            DynList = null;
                        }
                    }
                    _dynListUid = value;
                    OnPropertyChanged("DynListUid");
                }
            }
        }
        private Nullable<long> _dynListUid;
    
        [DataMember]
        public string DBTableFieldname
        {
            get { return _dBTableFieldname; }
            set
            {
                if (_dBTableFieldname != value)
                {
                    _dBTableFieldname = value;
                    OnPropertyChanged("DBTableFieldname");
                }
            }
        }
        private string _dBTableFieldname;
    
        [DataMember]
        public Nullable<long> ContextId
        {
            get { return _contextId; }
            set
            {
                if (_contextId != value)
                {
                    ChangeTracker.RecordOriginalValue("ContextId", _contextId);
                    if (!IsDeserializing)
                    {
                        if (Context != null && Context.ContextId != value)
                        {
                            Context = null;
                        }
                    }
                    _contextId = value;
                    OnPropertyChanged("ContextId");
                }
            }
        }
        private Nullable<long> _contextId;
    
        [DataMember]
        public bool IsDynListValue
        {
            get { return _isDynListValue; }
            set
            {
                if (_isDynListValue != value)
                {
                    _isDynListValue = value;
                    OnPropertyChanged("IsDynListValue");
                }
            }
        }
        private bool _isDynListValue;
    
        [DataMember]
        public bool IsFinancialInfo
        {
            get { return _isFinancialInfo; }
            set
            {
                if (_isFinancialInfo != value)
                {
                    _isFinancialInfo = value;
                    OnPropertyChanged("IsFinancialInfo");
                }
            }
        }
        private bool _isFinancialInfo;
    
        [DataMember]
        public bool IsRequired
        {
            get { return _isRequired; }
            set
            {
                if (_isRequired != value)
                {
                    _isRequired = value;
                    OnPropertyChanged("IsRequired");
                }
            }
        }
        private bool _isRequired;
    
        [DataMember]
        public bool IsKeyword
        {
            get { return _isKeyword; }
            set
            {
                if (_isKeyword != value)
                {
                    _isKeyword = value;
                    OnPropertyChanged("IsKeyword");
                }
            }
        }
        private bool _isKeyword;
    
        [DataMember]
        public string Format
        {
            get { return _format; }
            set
            {
                if (_format != value)
                {
                    _format = value;
                    OnPropertyChanged("Format");
                }
            }
        }
        private string _format;
    
        [DataMember]
        public bool IsFullTextInidex
        {
            get { return _isFullTextInidex; }
            set
            {
                if (_isFullTextInidex != value)
                {
                    _isFullTextInidex = value;
                    OnPropertyChanged("IsFullTextInidex");
                }
            }
        }
        private bool _isFullTextInidex;
    
        [DataMember]
        public bool DisplayOnResultList
        {
            get { return _displayOnResultList; }
            set
            {
                if (_displayOnResultList != value)
                {
                    _displayOnResultList = value;
                    OnPropertyChanged("DisplayOnResultList");
                }
            }
        }
        private bool _displayOnResultList;
    
        [DataMember]
        public Nullable<int> DisplayOrderResultList
        {
            get { return _displayOrderResultList; }
            set
            {
                if (_displayOrderResultList != value)
                {
                    _displayOrderResultList = value;
                    OnPropertyChanged("DisplayOrderResultList");
                }
            }
        }
        private Nullable<int> _displayOrderResultList;
    
        [DataMember]
        public Nullable<bool> DisplayOnExtResultList
        {
            get { return _displayOnExtResultList; }
            set
            {
                if (_displayOnExtResultList != value)
                {
                    _displayOnExtResultList = value;
                    OnPropertyChanged("DisplayOnExtResultList");
                }
            }
        }
        private Nullable<bool> _displayOnExtResultList;
    
        [DataMember]
        public long UpdateUserId
        {
            get { return _updateUserId; }
            set
            {
                if (_updateUserId != value)
                {
                    _updateUserId = value;
                    OnPropertyChanged("UpdateUserId");
                }
            }
        }
        private long _updateUserId;
    
        [DataMember]
        public System.DateTime UpdateDate
        {
            get { return _updateDate; }
            set
            {
                if (_updateDate != value)
                {
                    _updateDate = value;
                    OnPropertyChanged("UpdateDate");
                }
            }
        }
        private System.DateTime _updateDate;
    
        [DataMember]
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged("Label");
                }
            }
        }
        private string _label;
    
        [DataMember]
        public string LabelTranslationId
        {
            get { return _labelTranslationId; }
            set
            {
                if (_labelTranslationId != value)
                {
                    _labelTranslationId = value;
                    OnPropertyChanged("LabelTranslationId");
                }
            }
        }
        private string _labelTranslationId;
    
        [DataMember]
        public int Revision
        {
            get { return _revision; }
            set
            {
                if (_revision != value)
                {
                    _revision = value;
                    OnPropertyChanged("Revision");
                }
            }
        }
        private int _revision;
    
        [DataMember]
        public bool ActiveVersion
        {
            get { return _activeVersion; }
            set
            {
                if (_activeVersion != value)
                {
                    _activeVersion = value;
                    OnPropertyChanged("ActiveVersion");
                }
            }
        }
        private bool _activeVersion;
    
        [DataMember]
        public Nullable<int> DisplayOrderExtResultList
        {
            get { return _displayOrderExtResultList; }
            set
            {
                if (_displayOrderExtResultList != value)
                {
                    _displayOrderExtResultList = value;
                    OnPropertyChanged("DisplayOrderExtResultList");
                }
            }
        }
        private Nullable<int> _displayOrderExtResultList;
    
        [DataMember]
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }
        private string _comment;
    
        [DataMember]
        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active != value)
                {
                    _active = value;
                    OnPropertyChanged("Active");
                }
            }
        }
        private bool _active;
    
        [DataMember]
        public Nullable<long> RelatedAssetTypeID
        {
            get { return _relatedAssetTypeID; }
            set
            {
                if (_relatedAssetTypeID != value)
                {
                    _relatedAssetTypeID = value;
                    OnPropertyChanged("RelatedAssetTypeID");
                }
            }
        }
        private Nullable<long> _relatedAssetTypeID;
    
        [DataMember]
        public Nullable<long> RelatedAssetTypeAttributeID
        {
            get { return _relatedAssetTypeAttributeID; }
            set
            {
                if (_relatedAssetTypeAttributeID != value)
                {
                    _relatedAssetTypeAttributeID = value;
                    OnPropertyChanged("RelatedAssetTypeAttributeID");
                }
            }
        }
        private Nullable<long> _relatedAssetTypeAttributeID;
    
        [DataMember]
        public string ValidationExpr
        {
            get { return _validationExpr; }
            set
            {
                if (_validationExpr != value)
                {
                    _validationExpr = value;
                    OnPropertyChanged("ValidationExpr");
                }
            }
        }
        private string _validationExpr;
    
        [DataMember]
        public bool IsDescription
        {
            get { return _isDescription; }
            set
            {
                if (_isDescription != value)
                {
                    _isDescription = value;
                    OnPropertyChanged("IsDescription");
                }
            }
        }
        private bool _isDescription;
    
        [DataMember]
        public bool IsShownInGrid
        {
            get { return _isShownInGrid; }
            set
            {
                if (_isShownInGrid != value)
                {
                    _isShownInGrid = value;
                    OnPropertyChanged("IsShownInGrid");
                }
            }
        }
        private bool _isShownInGrid;
    
        [DataMember]
        public bool IsShownOnPanel
        {
            get { return _isShownOnPanel; }
            set
            {
                if (_isShownOnPanel != value)
                {
                    _isShownOnPanel = value;
                    OnPropertyChanged("IsShownOnPanel");
                }
            }
        }
        private bool _isShownOnPanel;
    
        [DataMember]
        public bool AllowEditConfig
        {
            get { return _allowEditConfig; }
            set
            {
                if (_allowEditConfig != value)
                {
                    _allowEditConfig = value;
                    OnPropertyChanged("AllowEditConfig");
                }
            }
        }
        private bool _allowEditConfig;
    
        [DataMember]
        public bool AllowEditValue
        {
            get { return _allowEditValue; }
            set
            {
                if (_allowEditValue != value)
                {
                    _allowEditValue = value;
                    OnPropertyChanged("AllowEditValue");
                }
            }
        }
        private bool _allowEditValue;
    
        [DataMember]
        public string ValidationMessage
        {
            get { return _validationMessage; }
            set
            {
                if (_validationMessage != value)
                {
                    _validationMessage = value;
                    OnPropertyChanged("ValidationMessage");
                }
            }
        }
        private string _validationMessage;
    
        [DataMember]
        public bool IsUsedForNames
        {
            get { return _isUsedForNames; }
            set
            {
                if (_isUsedForNames != value)
                {
                    _isUsedForNames = value;
                    OnPropertyChanged("IsUsedForNames");
                }
            }
        }
        private bool _isUsedForNames;
    
        [DataMember]
        public Nullable<int> NameGenOrder
        {
            get { return _nameGenOrder; }
            set
            {
                if (_nameGenOrder != value)
                {
                    _nameGenOrder = value;
                    OnPropertyChanged("NameGenOrder");
                }
            }
        }
        private Nullable<int> _nameGenOrder;
    
        [DataMember]
        public int DisplayOrder
        {
            get { return _displayOrder; }
            set
            {
                if (_displayOrder != value)
                {
                    _displayOrder = value;
                    OnPropertyChanged("DisplayOrder");
                }
            }
        }
        private int _displayOrder;
    
        [DataMember]
        public string CalculationFormula
        {
            get { return _calculationFormula; }
            set
            {
                if (_calculationFormula != value)
                {
                    _calculationFormula = value;
                    OnPropertyChanged("CalculationFormula");
                }
            }
        }
        private string _calculationFormula;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<AttributePanelAttribute> AttributePanelAttributes
        {
            get
            {
                if (_attributePanelAttributes == null)
                {
                    _attributePanelAttributes = new TrackableCollection<AttributePanelAttribute>();
                    _attributePanelAttributes.CollectionChanged += FixupAttributePanelAttributes;
    				_attributePanelAttributes.IsLoaded = false;
                }
                return _attributePanelAttributes;
            }
            set
            {
                if (!ReferenceEquals(_attributePanelAttributes, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_attributePanelAttributes != null)
                    {
                        _attributePanelAttributes.CollectionChanged -= FixupAttributePanelAttributes;
                    }
                    _attributePanelAttributes = value;
    				_attributePanelAttributes.IsLoaded = true;
                    if (_attributePanelAttributes != null)
                    {
                        _attributePanelAttributes.CollectionChanged += FixupAttributePanelAttributes;
                    }
                    OnNavigationPropertyChanged("AttributePanelAttributes");
                }
            }
        }
        private TrackableCollection<AttributePanelAttribute> _attributePanelAttributes;
    
        [DataMember]
        public Context Context
        {
            get { return _context; }
            set
            {
                if (!ReferenceEquals(_context, value))
                {
                    var previousValue = _context;
                    _context = value;
                    FixupContext(previousValue);
                    OnNavigationPropertyChanged("Context");
                }
            }
        }
        private Context _context;
    
        [DataMember]
        public DataType DataType
        {
            get { return _dataType; }
            set
            {
                if (!ReferenceEquals(_dataType, value))
                {
                    var previousValue = _dataType;
                    _dataType = value;
                    FixupDataType(previousValue);
                    OnNavigationPropertyChanged("DataType");
                }
            }
        }
        private DataType _dataType;
    
        [DataMember]
        public DynEntityConfig DynEntityConfig
        {
            get { return _dynEntityConfig; }
            set
            {
                if (!ReferenceEquals(_dynEntityConfig, value))
                {
                    var previousValue = _dynEntityConfig;
                    _dynEntityConfig = value;
                    FixupDynEntityConfig(previousValue);
                    OnNavigationPropertyChanged("DynEntityConfig");
                }
            }
        }
        private DynEntityConfig _dynEntityConfig;
    
        [DataMember]
        public TrackableCollection<DynListValue> DynListValues
        {
            get
            {
                if (_dynListValues == null)
                {
                    _dynListValues = new TrackableCollection<DynListValue>();
                    _dynListValues.CollectionChanged += FixupDynListValues;
    				_dynListValues.IsLoaded = false;
                }
                return _dynListValues;
            }
            set
            {
                if (!ReferenceEquals(_dynListValues, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_dynListValues != null)
                    {
                        _dynListValues.CollectionChanged -= FixupDynListValues;
                    }
                    _dynListValues = value;
    				_dynListValues.IsLoaded = true;
                    if (_dynListValues != null)
                    {
                        _dynListValues.CollectionChanged += FixupDynListValues;
                    }
                    OnNavigationPropertyChanged("DynListValues");
                }
            }
        }
        private TrackableCollection<DynListValue> _dynListValues;
    
        [DataMember]
        public DynList DynList
        {
            get { return _dynList; }
            set
            {
                if (!ReferenceEquals(_dynList, value))
                {
                    var previousValue = _dynList;
                    _dynList = value;
                    FixupDynList(previousValue);
                    OnNavigationPropertyChanged("DynList");
                }
            }
        }
        private DynList _dynList;

        #endregion

        #region ChangeTracking
    
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
        private ObjectChangeTracker _changeTracker;
    
        [DataMember]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        protected bool IsDeserializing { get; private set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        protected virtual void ClearNavigationProperties()
        {
            AttributePanelAttributes.Clear();
            Context = null;
            DataType = null;
            DynEntityConfig = null;
            DynListValues.Clear();
            DynList = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupContext(Context previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DynEntityAttribConfigs.Contains(this))
            {
                previousValue.DynEntityAttribConfigs.Remove(this);
            }
    
            if (Context != null)
            {
                if (!Context.DynEntityAttribConfigs.Contains(this))
                {
                    Context.DynEntityAttribConfigs.Add(this);
                }
    
                ContextId = Context.ContextId;
            }
            else if (!skipKeys)
            {
                ContextId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Context")
                    && (ChangeTracker.OriginalValues["Context"] == Context))
                {
                    ChangeTracker.OriginalValues.Remove("Context");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Context", previousValue);
                }
                if (Context != null && !Context.ChangeTracker.ChangeTrackingEnabled)
                {
                    Context.StartTracking();
                }
            }
        }
    
        private void FixupDataType(DataType previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DynEntityAttribConfigs.Contains(this))
            {
                previousValue.DynEntityAttribConfigs.Remove(this);
            }
    
            if (DataType != null)
            {
                if (!DataType.DynEntityAttribConfigs.Contains(this))
                {
                    DataType.DynEntityAttribConfigs.Add(this);
                }
    
                DataTypeUid = DataType.DataTypeUid;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("DataType")
                    && (ChangeTracker.OriginalValues["DataType"] == DataType))
                {
                    ChangeTracker.OriginalValues.Remove("DataType");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("DataType", previousValue);
                }
                if (DataType != null && !DataType.ChangeTracker.ChangeTrackingEnabled)
                {
                    DataType.StartTracking();
                }
            }
        }
    
        private void FixupDynEntityConfig(DynEntityConfig previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DynEntityAttribConfigs.Contains(this))
            {
                previousValue.DynEntityAttribConfigs.Remove(this);
            }
    
            if (DynEntityConfig != null)
            {
                if (!DynEntityConfig.DynEntityAttribConfigs.Contains(this))
                {
                    DynEntityConfig.DynEntityAttribConfigs.Add(this);
                }
    
                DynEntityConfigUid = DynEntityConfig.DynEntityConfigUid;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("DynEntityConfig")
                    && (ChangeTracker.OriginalValues["DynEntityConfig"] == DynEntityConfig))
                {
                    ChangeTracker.OriginalValues.Remove("DynEntityConfig");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("DynEntityConfig", previousValue);
                }
                if (DynEntityConfig != null && !DynEntityConfig.ChangeTracker.ChangeTrackingEnabled)
                {
                    DynEntityConfig.StartTracking();
                }
            }
        }
    
        private void FixupDynList(DynList previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DynEntityAttribConfigs.Contains(this))
            {
                previousValue.DynEntityAttribConfigs.Remove(this);
            }
    
            if (DynList != null)
            {
                if (!DynList.DynEntityAttribConfigs.Contains(this))
                {
                    DynList.DynEntityAttribConfigs.Add(this);
                }
    
                DynListUid = DynList.DynListUid;
            }
            else if (!skipKeys)
            {
                DynListUid = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("DynList")
                    && (ChangeTracker.OriginalValues["DynList"] == DynList))
                {
                    ChangeTracker.OriginalValues.Remove("DynList");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("DynList", previousValue);
                }
                if (DynList != null && !DynList.ChangeTracker.ChangeTrackingEnabled)
                {
                    DynList.StartTracking();
                }
            }
        }
    
        private void FixupAttributePanelAttributes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (AttributePanelAttribute item in e.NewItems)
                {
                    item.DynEntityAttribConfig = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("AttributePanelAttributes", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AttributePanelAttribute item in e.OldItems)
                {
                    if (ReferenceEquals(item.DynEntityAttribConfig, this))
                    {
                        item.DynEntityAttribConfig = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("AttributePanelAttributes", item);
                    }
                }
            }
        }
    
        private void FixupDynListValues(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DynListValue item in e.NewItems)
                {
                    item.DynEntityAttribConfig = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DynListValues", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DynListValue item in e.OldItems)
                {
                    if (ReferenceEquals(item.DynEntityAttribConfig, this))
                    {
                        item.DynEntityAttribConfig = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DynListValues", item);
                    }
                }
            }
        }

        #endregion

    }
}
