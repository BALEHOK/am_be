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
    [KnownType(typeof(LibraryTransactions))]
    public partial class LibraryTransactionType: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long LibTransactionTypeUid
        {
            get { return _libTransactionTypeUid; }
            set
            {
                if (_libTransactionTypeUid != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'LibTransactionTypeUid' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _libTransactionTypeUid = value;
                    OnPropertyChanged("LibTransactionTypeUid");
                }
            }
        }
        private long _libTransactionTypeUid;
    
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
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        private string _description;
    
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

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<LibraryTransactions> LibraryTransactions
        {
            get
            {
                if (_libraryTransactions == null)
                {
                    _libraryTransactions = new TrackableCollection<LibraryTransactions>();
                    _libraryTransactions.CollectionChanged += FixupLibraryTransactions;
    				_libraryTransactions.IsLoaded = false;
                }
                return _libraryTransactions;
            }
            set
            {
                if (!ReferenceEquals(_libraryTransactions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_libraryTransactions != null)
                    {
                        _libraryTransactions.CollectionChanged -= FixupLibraryTransactions;
                    }
                    _libraryTransactions = value;
    				_libraryTransactions.IsLoaded = true;
                    if (_libraryTransactions != null)
                    {
                        _libraryTransactions.CollectionChanged += FixupLibraryTransactions;
                    }
                    OnNavigationPropertyChanged("LibraryTransactions");
                }
            }
        }
        private TrackableCollection<LibraryTransactions> _libraryTransactions;

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
            LibraryTransactions.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupLibraryTransactions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (LibraryTransactions item in e.NewItems)
                {
                    item.LibraryTransactionType = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LibraryTransactions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (LibraryTransactions item in e.OldItems)
                {
                    if (ReferenceEquals(item.LibraryTransactionType, this))
                    {
                        item.LibraryTransactionType = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LibraryTransactions", item);
                    }
                }
            }
        }

        #endregion

    }
}
