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
    [KnownType(typeof(ReservedAsset))]
    public partial class Reservation: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long ReservationId
        {
            get { return _reservationId; }
            set
            {
                if (_reservationId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ReservationId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _reservationId = value;
                    OnPropertyChanged("ReservationId");
                }
            }
        }
        private long _reservationId;
    
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
        public long BorrowerId
        {
            get { return _borrowerId; }
            set
            {
                if (_borrowerId != value)
                {
                    _borrowerId = value;
                    OnPropertyChanged("BorrowerId");
                }
            }
        }
        private long _borrowerId;
    
        [DataMember]
        public System.DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged("StartDate");
                }
            }
        }
        private System.DateTime _startDate;
    
        [DataMember]
        public System.DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged("EndDate");
                }
            }
        }
        private System.DateTime _endDate;
    
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
        public short State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
        }
        private short _state;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<ReservedAsset> ReservedAssets
        {
            get
            {
                if (_reservedAssets == null)
                {
                    _reservedAssets = new TrackableCollection<ReservedAsset>();
                    _reservedAssets.CollectionChanged += FixupReservedAssets;
    				_reservedAssets.IsLoaded = false;
                }
                return _reservedAssets;
            }
            set
            {
                if (!ReferenceEquals(_reservedAssets, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_reservedAssets != null)
                    {
                        _reservedAssets.CollectionChanged -= FixupReservedAssets;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (ReservedAsset item in _reservedAssets)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _reservedAssets = value;
    				_reservedAssets.IsLoaded = true;
                    if (_reservedAssets != null)
                    {
                        _reservedAssets.CollectionChanged += FixupReservedAssets;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (ReservedAsset item in _reservedAssets)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("ReservedAssets");
                }
            }
        }
        private TrackableCollection<ReservedAsset> _reservedAssets;

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
            ReservedAssets.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupReservedAssets(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ReservedAsset item in e.NewItems)
                {
                    item.Reservation = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ReservedAssets", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ReservedAsset item in e.OldItems)
                {
                    if (ReferenceEquals(item.Reservation, this))
                    {
                        item.Reservation = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ReservedAssets", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }

        #endregion

    }
}
