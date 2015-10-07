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
    [KnownType(typeof(Report))]
    public partial class ReportField: IObjectWithChangeTracker, INotifyPropertyChanged, IDataEntity
    {
        #region Primitive Properties
    
        [DataMember]
        public long ReportFieldUid
        {
            get { return _reportFieldUid; }
            set
            {
                if (_reportFieldUid != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ReportFieldUid' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _reportFieldUid = value;
                    OnPropertyChanged("ReportFieldUid");
                }
            }
        }
        private long _reportFieldUid;
    
        [DataMember]
        public long ReportUid
        {
            get { return _reportUid; }
            set
            {
                if (_reportUid != value)
                {
                    ChangeTracker.RecordOriginalValue("ReportUid", _reportUid);
                    if (!IsDeserializing)
                    {
                        if (Report != null && Report.ReportUid != value)
                        {
                            Report = null;
                        }
                    }
                    _reportUid = value;
                    OnPropertyChanged("ReportUid");
                }
            }
        }
        private long _reportUid;
    
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
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }
        private bool _isVisible;
    
        [DataMember]
        public bool IsFilter
        {
            get { return _isFilter; }
            set
            {
                if (_isFilter != value)
                {
                    _isFilter = value;
                    OnPropertyChanged("IsFilter");
                }
            }
        }
        private bool _isFilter;

        #endregion

        #region Navigation Properties
    
        [DataMember]
        public Report Report
        {
            get { return _report; }
            set
            {
                if (!ReferenceEquals(_report, value))
                {
                    var previousValue = _report;
                    _report = value;
                    FixupReport(previousValue);
                    OnNavigationPropertyChanged("Report");
                }
            }
        }
        private Report _report;

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
    
        // This entity type is the dependent end in at least one association that performs cascade deletes.
        // This event handler will process notifications that occur when the principal end is deleted.
        internal void HandleCascadeDelete(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
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
            Report = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupReport(Report previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ReportField.Contains(this))
            {
                previousValue.ReportField.Remove(this);
            }
    
            if (Report != null)
            {
                if (!Report.ReportField.Contains(this))
                {
                    Report.ReportField.Add(this);
                }
    
                ReportUid = Report.ReportUid;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Report")
                    && (ChangeTracker.OriginalValues["Report"] == Report))
                {
                    ChangeTracker.OriginalValues.Remove("Report");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Report", previousValue);
                }
                if (Report != null && !Report.ChangeTracker.ChangeTrackingEnabled)
                {
                    Report.StartTracking();
                }
            }
        }

        #endregion

    }
}
