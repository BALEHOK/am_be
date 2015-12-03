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
    [KnownType(typeof(SearchQueryAttribute))]
    public partial class SearchQuery: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Id' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        private long _id;
    
        [DataMember]
        public System.Guid SearchId
        {
            get { return _searchId; }
            set
            {
                if (_searchId != value)
                {
                    _searchId = value;
                    OnPropertyChanged("SearchId");
                }
            }
        }
        private System.Guid _searchId;
    
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
        public long AssetTypeId
        {
            get { return _assetTypeId; }
            set
            {
                if (_assetTypeId != value)
                {
                    _assetTypeId = value;
                    OnPropertyChanged("AssetTypeId");
                }
            }
        }
        private long _assetTypeId;
    
        [DataMember]
        public byte Context
        {
            get { return _context; }
            set
            {
                if (_context != value)
                {
                    _context = value;
                    OnPropertyChanged("Context");
                }
            }
        }
        private byte _context;
    
        [DataMember]
        public System.DateTime Created
        {
            get { return _created; }
            set
            {
                if (_created != value)
                {
                    _created = value;
                    OnPropertyChanged("Created");
                }
            }
        }
        private System.DateTime _created;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public TrackableCollection<SearchQueryAttribute> SearchQueryAttributes
        {
            get
            {
                if (_searchQueryAttributes == null)
                {
                    _searchQueryAttributes = new TrackableCollection<SearchQueryAttribute>();
                    _searchQueryAttributes.CollectionChanged += FixupSearchQueryAttributes;
    				_searchQueryAttributes.IsLoaded = false;
                }
                return _searchQueryAttributes;
            }
            set
            {
                if (!ReferenceEquals(_searchQueryAttributes, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_searchQueryAttributes != null)
                    {
                        _searchQueryAttributes.CollectionChanged -= FixupSearchQueryAttributes;
                    }
                    _searchQueryAttributes = value;
    				_searchQueryAttributes.IsLoaded = true;
                    if (_searchQueryAttributes != null)
                    {
                        _searchQueryAttributes.CollectionChanged += FixupSearchQueryAttributes;
                    }
                    OnNavigationPropertyChanged("SearchQueryAttributes");
                }
            }
        }
        private TrackableCollection<SearchQueryAttribute> _searchQueryAttributes;

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
            SearchQueryAttributes.Clear();
        }

        #endregion

        #region Association Fixup
    
        private void FixupSearchQueryAttributes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (SearchQueryAttribute item in e.NewItems)
                {
                    item.SearchQuery = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("SearchQueryAttributes", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SearchQueryAttribute item in e.OldItems)
                {
                    if (ReferenceEquals(item.SearchQuery, this))
                    {
                        item.SearchQuery = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("SearchQueryAttributes", item);
                    }
                }
            }
        }

        #endregion

    }
}