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
    [KnownType(typeof(DataType))]
    public partial class ValidationOperand: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long OperandUid
        {
            get { return _operandUid; }
            set
            {
                if (_operandUid != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'OperandUid' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _operandUid = value;
                    OnPropertyChanged("OperandUid");
                }
            }
        }
        private long _operandUid;
    
        [DataMember]
        public long ValidationOperatorUid
        {
            get { return _validationOperatorUid; }
            set
            {
                if (_validationOperatorUid != value)
                {
                    ChangeTracker.RecordOriginalValue("ValidationOperatorUid", _validationOperatorUid);
                    _validationOperatorUid = value;
                    OnPropertyChanged("ValidationOperatorUid");
                }
            }
        }
        private long _validationOperatorUid;
    
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
        public string Alias
        {
            get { return _alias; }
            set
            {
                if (_alias != value)
                {
                    _alias = value;
                    OnPropertyChanged("Alias");
                }
            }
        }
        private string _alias;

        #endregion

        #region Navigation Properties
    
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
            DataType = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupDataType(DataType previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (DataType != null)
            {
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

        #endregion

    }
}
