//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppFramework.DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class DataType
    {
        public DataType()
        {
            this.Context = new HashSet<Context>();
            this.SearchOperators = new HashSet<SearchOperators>();
            this.ValidationList = new HashSet<ValidationList>();
            this.DynEntityAttribConfigs = new HashSet<DynEntityAttribConfig>();
            this.DynLists = new HashSet<DynList>();
        }
    
        public long DataTypeUid { get; set; }
        public string Name { get; set; }
        public string NameTranslationId { get; set; }
        public string DBDataType { get; set; }
        public string FrameworkDataType { get; set; }
        public string Comment { get; set; }
        public long UpdateUserId { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public Nullable<int> StringSize { get; set; }
        public Nullable<int> DefaultValueID { get; set; }
        public string ValidationExpr { get; set; }
        public bool IsInternal { get; set; }
        public bool IsEditable { get; set; }
        public string ValidationMessage { get; set; }
    
        public virtual ICollection<Context> Context { get; set; }
        public virtual ICollection<SearchOperators> SearchOperators { get; set; }
        public virtual ICollection<ValidationList> ValidationList { get; set; }
        public virtual ICollection<DynEntityAttribConfig> DynEntityAttribConfigs { get; set; }
        public virtual ICollection<DynList> DynLists { get; set; }
    }
}
