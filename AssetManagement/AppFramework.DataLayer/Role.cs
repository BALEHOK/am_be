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
    
    public partial class Role
    {
        public Role()
        {
            this.UserInRole = new HashSet<UserInRole>();
        }
    
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<UserInRole> UserInRole { get; set; }
    }
}
