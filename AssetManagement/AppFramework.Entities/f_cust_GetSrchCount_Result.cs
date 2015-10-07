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
    
    public partial class f_cust_GetSrchCount_Result : INotifyComplexPropertyChanging, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public Nullable<long> SearchId
        {
            get { return _searchId; }
            set
            {
                if (_searchId != value)
                {
                    OnComplexPropertyChanging();
                    _searchId = value;
                    OnPropertyChanged("SearchId");
                }
            }
        }
        private Nullable<long> _searchId;
    
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
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    OnComplexPropertyChanging();
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        private string _type;
    
        [DataMember]
        public Nullable<long> id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    OnComplexPropertyChanging();
                    _id = value;
                    OnPropertyChanged("id");
                }
            }
        }
        private Nullable<long> _id;
    
        [DataMember]
        public Nullable<int> Count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    OnComplexPropertyChanging();
                    _count = value;
                    OnPropertyChanged("Count");
                }
            }
        }
        private Nullable<int> _count;

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
    
        public static void RecordComplexOriginalValues(String parentPropertyName, f_cust_GetSrchCount_Result complexObject, ObjectChangeTracker changeTracker)
        {
            if (String.IsNullOrEmpty(parentPropertyName))
            {
                throw new ArgumentException("String parameter cannot be null or empty.", "parentPropertyName");
            }
    
            if (changeTracker == null)
            {
                throw new ArgumentNullException("changeTracker");
            }
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.SearchId", parentPropertyName), complexObject == null ? null : (object)complexObject.SearchId);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.UserId", parentPropertyName), complexObject == null ? null : (object)complexObject.UserId);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.Type", parentPropertyName), complexObject == null ? null : (object)complexObject.Type);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.id", parentPropertyName), complexObject == null ? null : (object)complexObject.id);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.Count", parentPropertyName), complexObject == null ? null : (object)complexObject.Count);
        }

        #endregion

    }
}
