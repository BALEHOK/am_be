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
    [KnownType(typeof(BatchAction))]
    [KnownType(typeof(BatchSchedule))]
    public partial class BatchJob: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long BatchUid
        {
            get { return _batchUid; }
            set
            {
                if (_batchUid != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'BatchUid' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _batchUid = value;
                    OnPropertyChanged("BatchUid");
                }
            }
        }
        private long _batchUid;
    
        [DataMember]
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }
        private string _title;
    
        [DataMember]
        public long OwnerId
        {
            get { return _ownerId; }
            set
            {
                if (_ownerId != value)
                {
                    _ownerId = value;
                    OnPropertyChanged("OwnerId");
                }
            }
        }
        private long _ownerId;
    
        [DataMember]
        public Nullable<System.DateTime> StartDate
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
        private Nullable<System.DateTime> _startDate;
    
        [DataMember]
        public Nullable<System.DateTime> EndDate
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
        private Nullable<System.DateTime> _endDate;
    
        [DataMember]
        public short Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        private short _status;
    
        [DataMember]
        public bool SkipErrors
        {
            get { return _skipErrors; }
            set
            {
                if (_skipErrors != value)
                {
                    _skipErrors = value;
                    OnPropertyChanged("SkipErrors");
                }
            }
        }
        private bool _skipErrors;
    
        [DataMember]
        public short ExecuteOn
        {
            get { return _executeOn; }
            set
            {
                if (_executeOn != value)
                {
                    _executeOn = value;
                    OnPropertyChanged("ExecuteOn");
                }
            }
        }
        private short _executeOn;
    
        [DataMember]
        public Nullable<long> BatchScheduleId
        {
            get { return _batchScheduleId; }
            set
            {
                if (_batchScheduleId != value)
                {
                    ChangeTracker.RecordOriginalValue("BatchScheduleId", _batchScheduleId);
                    if (!IsDeserializing)
                    {
                        if (BatchSchedule != null && BatchSchedule.ScheduleId != value)
                        {
                            BatchSchedule = null;
                        }
                    }
                    _batchScheduleId = value;
                    OnPropertyChanged("BatchScheduleId");
                }
            }
        }
        private Nullable<long> _batchScheduleId;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<BatchAction> BatchActions
        {
            get
            {
                if (_batchActions == null)
                {
                    _batchActions = new TrackableCollection<BatchAction>();
                    _batchActions.CollectionChanged += FixupBatchActions;
    				_batchActions.IsLoaded = false;
                }
                return _batchActions;
            }
            set
            {
                if (!ReferenceEquals(_batchActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_batchActions != null)
                    {
                        _batchActions.CollectionChanged -= FixupBatchActions;
                    }
                    _batchActions = value;
    				_batchActions.IsLoaded = true;
                    if (_batchActions != null)
                    {
                        _batchActions.CollectionChanged += FixupBatchActions;
                    }
                    OnNavigationPropertyChanged("BatchActions");
                }
            }
        }
        private TrackableCollection<BatchAction> _batchActions;
    
        [DataMember]
        public BatchSchedule BatchSchedule
        {
            get { return _batchSchedule; }
            set
            {
                if (!ReferenceEquals(_batchSchedule, value))
                {
                    var previousValue = _batchSchedule;
                    _batchSchedule = value;
                    FixupBatchSchedule(previousValue);
                    OnNavigationPropertyChanged("BatchSchedule");
                }
            }
        }
        private BatchSchedule _batchSchedule;

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
            BatchActions.Clear();
            BatchSchedule = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupBatchSchedule(BatchSchedule previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (BatchSchedule != null)
            {
                BatchScheduleId = BatchSchedule.ScheduleId;
            }
    
            else if (!skipKeys)
            {
                BatchScheduleId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("BatchSchedule")
                    && (ChangeTracker.OriginalValues["BatchSchedule"] == BatchSchedule))
                {
                    ChangeTracker.OriginalValues.Remove("BatchSchedule");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("BatchSchedule", previousValue);
                }
                if (BatchSchedule != null && !BatchSchedule.ChangeTracker.ChangeTrackingEnabled)
                {
                    BatchSchedule.StartTracking();
                }
            }
        }
    
        private void FixupBatchActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (BatchAction item in e.NewItems)
                {
                    item.BatchJob = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("BatchActions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (BatchAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.BatchJob, this))
                    {
                        item.BatchJob = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("BatchActions", item);
                    }
                }
            }
        }

        #endregion

    }
}
