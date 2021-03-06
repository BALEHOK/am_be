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
    
    public partial class f_cust_SearchByTypeContext_Result : INotifyComplexPropertyChanging, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public long IndexUid
        {
            get { return _indexUid; }
            set
            {
                if (_indexUid != value)
                {
                    OnComplexPropertyChanging();
                    _indexUid = value;
                    OnPropertyChanged("IndexUid");
                }
            }
        }
        private long _indexUid;
    
        [DataMember]
        public long DynEntityUid
        {
            get { return _dynEntityUid; }
            set
            {
                if (_dynEntityUid != value)
                {
                    OnComplexPropertyChanging();
                    _dynEntityUid = value;
                    OnPropertyChanged("DynEntityUid");
                }
            }
        }
        private long _dynEntityUid;
    
        [DataMember]
        public string BarCode
        {
            get { return _barCode; }
            set
            {
                if (_barCode != value)
                {
                    OnComplexPropertyChanging();
                    _barCode = value;
                    OnPropertyChanged("BarCode");
                }
            }
        }
        private string _barCode;
    
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    OnComplexPropertyChanging();
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        private string _name;
    
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    OnComplexPropertyChanging();
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        private string _description;
    
        [DataMember]
        public string Keywords
        {
            get { return _keywords; }
            set
            {
                if (_keywords != value)
                {
                    OnComplexPropertyChanging();
                    _keywords = value;
                    OnPropertyChanged("Keywords");
                }
            }
        }
        private string _keywords;
    
        [DataMember]
        public string EntityConfigKeywords
        {
            get { return _entityConfigKeywords; }
            set
            {
                if (_entityConfigKeywords != value)
                {
                    OnComplexPropertyChanging();
                    _entityConfigKeywords = value;
                    OnPropertyChanged("EntityConfigKeywords");
                }
            }
        }
        private string _entityConfigKeywords;
    
        [DataMember]
        public string AllAttrib2IndexValues
        {
            get { return _allAttrib2IndexValues; }
            set
            {
                if (_allAttrib2IndexValues != value)
                {
                    OnComplexPropertyChanging();
                    _allAttrib2IndexValues = value;
                    OnPropertyChanged("AllAttrib2IndexValues");
                }
            }
        }
        private string _allAttrib2IndexValues;
    
        [DataMember]
        public string AllContextAttribValues
        {
            get { return _allContextAttribValues; }
            set
            {
                if (_allContextAttribValues != value)
                {
                    OnComplexPropertyChanging();
                    _allContextAttribValues = value;
                    OnPropertyChanged("AllContextAttribValues");
                }
            }
        }
        private string _allContextAttribValues;
    
        [DataMember]
        public string AllAttribValues
        {
            get { return _allAttribValues; }
            set
            {
                if (_allAttribValues != value)
                {
                    OnComplexPropertyChanging();
                    _allAttribValues = value;
                    OnPropertyChanged("AllAttribValues");
                }
            }
        }
        private string _allAttribValues;
    
        [DataMember]
        public string CategoryKeywords
        {
            get { return _categoryKeywords; }
            set
            {
                if (_categoryKeywords != value)
                {
                    OnComplexPropertyChanging();
                    _categoryKeywords = value;
                    OnPropertyChanged("CategoryKeywords");
                }
            }
        }
        private string _categoryKeywords;
    
        [DataMember]
        public string TaxonomyKeywords
        {
            get { return _taxonomyKeywords; }
            set
            {
                if (_taxonomyKeywords != value)
                {
                    OnComplexPropertyChanging();
                    _taxonomyKeywords = value;
                    OnPropertyChanged("TaxonomyKeywords");
                }
            }
        }
        private string _taxonomyKeywords;
    
        [DataMember]
        public string User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    OnComplexPropertyChanging();
                    _user = value;
                    OnPropertyChanged("User");
                }
            }
        }
        private string _user;
    
        [DataMember]
        public long LocationUid
        {
            get { return _locationUid; }
            set
            {
                if (_locationUid != value)
                {
                    OnComplexPropertyChanging();
                    _locationUid = value;
                    OnPropertyChanged("LocationUid");
                }
            }
        }
        private long _locationUid;
    
        [DataMember]
        public string Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    OnComplexPropertyChanging();
                    _location = value;
                    OnPropertyChanged("Location");
                }
            }
        }
        private string _location;
    
        [DataMember]
        public string Department
        {
            get { return _department; }
            set
            {
                if (_department != value)
                {
                    OnComplexPropertyChanging();
                    _department = value;
                    OnPropertyChanged("Department");
                }
            }
        }
        private string _department;
    
        [DataMember]
        public long DynEntityConfigUid
        {
            get { return _dynEntityConfigUid; }
            set
            {
                if (_dynEntityConfigUid != value)
                {
                    OnComplexPropertyChanging();
                    _dynEntityConfigUid = value;
                    OnPropertyChanged("DynEntityConfigUid");
                }
            }
        }
        private long _dynEntityConfigUid;
    
        [DataMember]
        public System.DateTime UpdateDate
        {
            get { return _updateDate; }
            set
            {
                if (_updateDate != value)
                {
                    OnComplexPropertyChanging();
                    _updateDate = value;
                    OnPropertyChanged("UpdateDate");
                }
            }
        }
        private System.DateTime _updateDate;
    
        [DataMember]
        public string CategoryUids
        {
            get { return _categoryUids; }
            set
            {
                if (_categoryUids != value)
                {
                    OnComplexPropertyChanging();
                    _categoryUids = value;
                    OnPropertyChanged("CategoryUids");
                }
            }
        }
        private string _categoryUids;
    
        [DataMember]
        public string TaxonomyUids
        {
            get { return _taxonomyUids; }
            set
            {
                if (_taxonomyUids != value)
                {
                    OnComplexPropertyChanging();
                    _taxonomyUids = value;
                    OnPropertyChanged("TaxonomyUids");
                }
            }
        }
        private string _taxonomyUids;
    
        [DataMember]
        public long OwnerId
        {
            get { return _ownerId; }
            set
            {
                if (_ownerId != value)
                {
                    OnComplexPropertyChanging();
                    _ownerId = value;
                    OnPropertyChanged("OwnerId");
                }
            }
        }
        private long _ownerId;
    
        [DataMember]
        public long DepartmentId
        {
            get { return _departmentId; }
            set
            {
                if (_departmentId != value)
                {
                    OnComplexPropertyChanging();
                    _departmentId = value;
                    OnPropertyChanged("DepartmentId");
                }
            }
        }
        private long _departmentId;
    
        [DataMember]
        public long DynEntityId
        {
            get { return _dynEntityId; }
            set
            {
                if (_dynEntityId != value)
                {
                    OnComplexPropertyChanging();
                    _dynEntityId = value;
                    OnPropertyChanged("DynEntityId");
                }
            }
        }
        private long _dynEntityId;
    
        [DataMember]
        public string TaxonomyItemsIds
        {
            get { return _taxonomyItemsIds; }
            set
            {
                if (_taxonomyItemsIds != value)
                {
                    OnComplexPropertyChanging();
                    _taxonomyItemsIds = value;
                    OnPropertyChanged("TaxonomyItemsIds");
                }
            }
        }
        private string _taxonomyItemsIds;
    
        [DataMember]
        public long DynEntityConfigId
        {
            get { return _dynEntityConfigId; }
            set
            {
                if (_dynEntityConfigId != value)
                {
                    OnComplexPropertyChanging();
                    _dynEntityConfigId = value;
                    OnPropertyChanged("DynEntityConfigId");
                }
            }
        }
        private long _dynEntityConfigId;
    
        [DataMember]
        public string DisplayValues
        {
            get { return _displayValues; }
            set
            {
                if (_displayValues != value)
                {
                    OnComplexPropertyChanging();
                    _displayValues = value;
                    OnPropertyChanged("DisplayValues");
                }
            }
        }
        private string _displayValues;
    
        [DataMember]
        public string DisplayExtValues
        {
            get { return _displayExtValues; }
            set
            {
                if (_displayExtValues != value)
                {
                    OnComplexPropertyChanging();
                    _displayExtValues = value;
                    OnPropertyChanged("DisplayExtValues");
                }
            }
        }
        private string _displayExtValues;
    
        [DataMember]
        public Nullable<long> UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    OnComplexPropertyChanging();
                    _userId = value;
                    OnPropertyChanged("UserId");
                }
            }
        }
        private Nullable<long> _userId;
    
        [DataMember]
        public Nullable<int> rownumber
        {
            get { return _rownumber; }
            set
            {
                if (_rownumber != value)
                {
                    OnComplexPropertyChanging();
                    _rownumber = value;
                    OnPropertyChanged("rownumber");
                }
            }
        }
        private Nullable<int> _rownumber;

        #endregion

        #region ChangeTracking
    
        private void OnComplexPropertyChanging()
        {
            if (_complexPropertyChanging != null)
            {
                _complexPropertyChanging(this, new EventArgs());
            }
        }
    
        event EventHandler INotifyComplexPropertyChanging.ComplexPropertyChanging { add { _complexPropertyChanging += value; } remove { _complexPropertyChanging -= value; } }
        private event EventHandler _complexPropertyChanging;
    
        private void OnPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
    
        public static void RecordComplexOriginalValues(String parentPropertyName, f_cust_SearchByTypeContext_Result complexObject, ObjectChangeTracker changeTracker)
        {
            if (String.IsNullOrEmpty(parentPropertyName))
            {
                throw new ArgumentException("String parameter cannot be null or empty.", "parentPropertyName");
            }
    
            if (changeTracker == null)
            {
                throw new ArgumentNullException("changeTracker");
            }
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.IndexUid", parentPropertyName), complexObject == null ? null : (object)complexObject.IndexUid);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.DynEntityUid", parentPropertyName), complexObject == null ? null : (object)complexObject.DynEntityUid);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.BarCode", parentPropertyName), complexObject == null ? null : (object)complexObject.BarCode);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.Name", parentPropertyName), complexObject == null ? null : (object)complexObject.Name);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.Description", parentPropertyName), complexObject == null ? null : (object)complexObject.Description);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.Keywords", parentPropertyName), complexObject == null ? null : (object)complexObject.Keywords);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.EntityConfigKeywords", parentPropertyName), complexObject == null ? null : (object)complexObject.EntityConfigKeywords);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.AllAttrib2IndexValues", parentPropertyName), complexObject == null ? null : (object)complexObject.AllAttrib2IndexValues);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.AllContextAttribValues", parentPropertyName), complexObject == null ? null : (object)complexObject.AllContextAttribValues);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.AllAttribValues", parentPropertyName), complexObject == null ? null : (object)complexObject.AllAttribValues);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.CategoryKeywords", parentPropertyName), complexObject == null ? null : (object)complexObject.CategoryKeywords);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.TaxonomyKeywords", parentPropertyName), complexObject == null ? null : (object)complexObject.TaxonomyKeywords);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.User", parentPropertyName), complexObject == null ? null : (object)complexObject.User);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.LocationUid", parentPropertyName), complexObject == null ? null : (object)complexObject.LocationUid);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.Location", parentPropertyName), complexObject == null ? null : (object)complexObject.Location);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.Department", parentPropertyName), complexObject == null ? null : (object)complexObject.Department);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.DynEntityConfigUid", parentPropertyName), complexObject == null ? null : (object)complexObject.DynEntityConfigUid);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.UpdateDate", parentPropertyName), complexObject == null ? null : (object)complexObject.UpdateDate);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.CategoryUids", parentPropertyName), complexObject == null ? null : (object)complexObject.CategoryUids);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.TaxonomyUids", parentPropertyName), complexObject == null ? null : (object)complexObject.TaxonomyUids);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.OwnerId", parentPropertyName), complexObject == null ? null : (object)complexObject.OwnerId);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.DepartmentId", parentPropertyName), complexObject == null ? null : (object)complexObject.DepartmentId);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.DynEntityId", parentPropertyName), complexObject == null ? null : (object)complexObject.DynEntityId);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.TaxonomyItemsIds", parentPropertyName), complexObject == null ? null : (object)complexObject.TaxonomyItemsIds);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.DynEntityConfigId", parentPropertyName), complexObject == null ? null : (object)complexObject.DynEntityConfigId);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.DisplayValues", parentPropertyName), complexObject == null ? null : (object)complexObject.DisplayValues);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.DisplayExtValues", parentPropertyName), complexObject == null ? null : (object)complexObject.DisplayExtValues);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.UserId", parentPropertyName), complexObject == null ? null : (object)complexObject.UserId);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.rownumber", parentPropertyName), complexObject == null ? null : (object)complexObject.rownumber);
        }

        #endregion

    }
}
