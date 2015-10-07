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
    [KnownType(typeof(LibraryTransactionType))]
    public partial class LibraryTransactions: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long LibraryTransactionUId
        {
            get { return _libraryTransactionUId; }
            set
            {
                if (_libraryTransactionUId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'LibraryTransactionUId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _libraryTransactionUId = value;
                    OnPropertyChanged("LibraryTransactionUId");
                }
            }
        }
        private long _libraryTransactionUId;
    
        [DataMember]
        public long DynEntityId
        {
            get { return _dynEntityId; }
            set
            {
                if (_dynEntityId != value)
                {
                    _dynEntityId = value;
                    OnPropertyChanged("DynEntityId");
                }
            }
        }
        private long _dynEntityId;
    
        [DataMember]
        public long LibraryTransactionTypeUid
        {
            get { return _libraryTransactionTypeUid; }
            set
            {
                if (_libraryTransactionTypeUid != value)
                {
                    ChangeTracker.RecordOriginalValue("LibraryTransactionTypeUid", _libraryTransactionTypeUid);
                    if (!IsDeserializing)
                    {
                        if (LibraryTransactionType != null && LibraryTransactionType.LibTransactionTypeUid != value)
                        {
                            LibraryTransactionType = null;
                        }
                    }
                    _libraryTransactionTypeUid = value;
                    OnPropertyChanged("LibraryTransactionTypeUid");
                }
            }
        }
        private long _libraryTransactionTypeUid;
    
        [DataMember]
        public System.DateTime TransactionDate
        {
            get { return _transactionDate; }
            set
            {
                if (_transactionDate != value)
                {
                    _transactionDate = value;
                    OnPropertyChanged("TransactionDate");
                }
            }
        }
        private System.DateTime _transactionDate;
    
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
        public int UpdateUserId
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
        private int _updateUserId;
    
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

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public LibraryTransactionType LibraryTransactionType
        {
            get { return _libraryTransactionType; }
            set
            {
                if (!ReferenceEquals(_libraryTransactionType, value))
                {
                    var previousValue = _libraryTransactionType;
                    _libraryTransactionType = value;
                    FixupLibraryTransactionType(previousValue);
                    OnNavigationPropertyChanged("LibraryTransactionType");
                }
            }
        }
        private LibraryTransactionType _libraryTransactionType;

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
            LibraryTransactionType = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupLibraryTransactionType(LibraryTransactionType previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.LibraryTransactions.Contains(this))
            {
                previousValue.LibraryTransactions.Remove(this);
            }
    
            if (LibraryTransactionType != null)
            {
                if (!LibraryTransactionType.LibraryTransactions.Contains(this))
                {
                    LibraryTransactionType.LibraryTransactions.Add(this);
                }
    
                LibraryTransactionTypeUid = LibraryTransactionType.LibTransactionTypeUid;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("LibraryTransactionType")
                    && (ChangeTracker.OriginalValues["LibraryTransactionType"] == LibraryTransactionType))
                {
                    ChangeTracker.OriginalValues.Remove("LibraryTransactionType");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("LibraryTransactionType", previousValue);
                }
                if (LibraryTransactionType != null && !LibraryTransactionType.ChangeTracker.ChangeTrackingEnabled)
                {
                    LibraryTransactionType.StartTracking();
                }
            }
        }

        #endregion

    }
}
