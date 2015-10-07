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
    
    public partial class f_cust_GetSqlServerAgentJobs_Result : INotifyComplexPropertyChanging, INotifyPropertyChanged
    {
        #region Primitive Properties
    
        [DataMember]
        public System.Guid job_id
        {
            get { return _job_id; }
            set
            {
                if (_job_id != value)
                {
                    OnComplexPropertyChanging();
                    _job_id = value;
                    OnPropertyChanged("job_id");
                }
            }
        }
        private System.Guid _job_id;
    
        [DataMember]
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    OnComplexPropertyChanging();
                    _name = value;
                    OnPropertyChanged("name");
                }
            }
        }
        private string _name;

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
    
        public static void RecordComplexOriginalValues(String parentPropertyName, f_cust_GetSqlServerAgentJobs_Result complexObject, ObjectChangeTracker changeTracker)
        {
            if (String.IsNullOrEmpty(parentPropertyName))
            {
                throw new ArgumentException("String parameter cannot be null or empty.", "parentPropertyName");
            }
    
            if (changeTracker == null)
            {
                throw new ArgumentNullException("changeTracker");
            }
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.job_id", parentPropertyName), complexObject == null ? null : (object)complexObject.job_id);
            changeTracker.RecordOriginalValue(String.Format(CultureInfo.InvariantCulture, "{0}.name", parentPropertyName), complexObject == null ? null : (object)complexObject.name);
        }

        #endregion

    }
}
