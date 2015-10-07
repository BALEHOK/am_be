using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.ConstantsEnumerators
{
    /// <summary>
    /// Contains predefined lists of permissions 
    /// to simplify the filtering of assets lists
    /// </summary>
    public static class PermissionsList
    {
        /// <summary>
        /// Gets the list with permissions 
        /// for reading the common information (Rxxx)
        /// and with any other permissions
        /// </summary>
        public static List<Enumerators.Permissions> AtLeastReadNormal
        {
            get
            {
                List<Enumerators.Permissions> returnList = new List<Enumerators.Permissions>();                
                returnList.Add(Enumerators.Permissions.RDRD);
                returnList.Add(Enumerators.Permissions.RDDD);
                returnList.Add(Enumerators.Permissions.RDDW);
                returnList.Add(Enumerators.Permissions.RDRW);
                returnList.Add(Enumerators.Permissions.RWDD);
                returnList.Add(Enumerators.Permissions.RWDW);
                returnList.Add(Enumerators.Permissions.RWRD);
                returnList.Add(Enumerators.Permissions.RWRW);
                return returnList;
            }
        }

        /// <summary>
        /// Gets the list with permissions 
        /// for reading and writing the common information (RWxx)
        /// and with any other permissions
        /// </summary>
        public static List<Enumerators.Permissions> ReadWriteNormalOnly
        {
            get
            {
                List<Enumerators.Permissions> returnList = new List<Enumerators.Permissions>();       
                returnList.Add(Enumerators.Permissions.RWDD);      
                return returnList;
            }
        }

        public static List<Enumerators.Permissions> ReadWriteNormalReadFinancialOnly
        {
            get
            {
                List<Enumerators.Permissions> returnList = new List<Enumerators.Permissions>();
                returnList.Add(Enumerators.Permissions.RWDD);      
                returnList.Add(Enumerators.Permissions.RWRD);    
                return returnList;
            }
        }
    }
}
