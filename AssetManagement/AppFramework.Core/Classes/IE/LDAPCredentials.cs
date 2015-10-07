using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE
{
    /// <summary>
    /// Describes the parameters for connecting to AD
    /// </summary>
    public class LDAPCredentials : ActionParameters<string, string>
    {
        /// <summary>
        /// AD domain
        /// </summary>
        public string Domain
        {
            get
            {
                return this["Domain"];
            }
            set
            {
                this["Domain"] = value;
            }
        }

        /// <summary>
        /// AD user name
        /// </summary>
        public string UserName
        {
            get
            {
                return this["UserName"];
            }
            set
            {
                this["UserName"] = value;
            }
        }

        /// <summary>
        /// AD user password
        /// </summary>
        public string Password
        {
            get
            {
                return this["Password"];
            }
            set
            {
                this["Password"] = value;
            }
        }

        public LDAPCredentials() 
            : base() { }
    }
}
