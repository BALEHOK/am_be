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
    
    public partial class ZipCode
    {
        public ZipCode()
        {
            this.Place2Zip = new HashSet<Place2Zip>();
        }
    
        public long ZipId { get; set; }
        public string Code { get; set; }
    
        public virtual ICollection<Place2Zip> Place2Zip { get; set; }
    }
}
