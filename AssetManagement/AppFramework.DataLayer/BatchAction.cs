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
    
    public partial class BatchAction
    {
        public long BatchActionUid { get; set; }
        public long BatchUid { get; set; }
        public int ActionType { get; set; }
        public long OrderId { get; set; }
        public short Status { get; set; }
        public string ErrorMessage { get; set; }
        public string ActionParams { get; set; }
        public bool IsMandatory { get; set; }
    
        public virtual BatchJob BatchJob { get; set; }
    }
}
