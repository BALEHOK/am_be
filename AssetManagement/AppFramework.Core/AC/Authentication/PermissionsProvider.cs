using System;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.AC.Authentication
{

    /// <summary>
    /// Collection of user rights 
    /// which determines the permissions
    /// for specific asset
    /// </summary>
    public enum Permission
    {

        /// <summary>
        /// 1234
        /// 1 - read normal info
        /// 2 - write normal info
        /// 3 - read financial info
        /// 4 - write financial info
        /// </summary>
        Unknown = -1,
        DDDD, //0000 - 0
        DDDW, //0001 - 1
        DDRD, //0010 - 2
        DDRW, //0011 - 3

        DWDD, //0100 - 4
        DWDW, //0101 - 5
        DWRD, //0110 - 6
        DWRW, //0111 - 7

        RDDD, //1000 - 8
        RDDW, //1001 - 9
        RDRD, //1010 - 10 
        RDRW, //1011 - 11   

        RWDD, //1100 - 12
        RWDW, //1101 - 13
        RWRD, //1110 - 14
        RWRW, //1111 - 15

        ReadWriteDelete = 16 // 10000
    }


    /// <summary>
    /// Contains predefined lists of permissions 
    /// to simplify the filtering of assets lists
    /// </summary>
    public static class PermissionsProvider
    {

        private static readonly Dictionary<byte, Permission> permissions =
            new Dictionary<byte, Permission>(17);

        private static readonly Dictionary<Permission, string> permissionFriendlyNames =
            new Dictionary<Permission, string>(5);

        static PermissionsProvider()
        {
            permissions.Add(0, Permission.DDDD);
            permissions.Add(1, Permission.DDDW);
            permissions.Add(2, Permission.DDRD);
            permissions.Add(3, Permission.DDRW);

            permissions.Add(4, Permission.DWDD);
            permissions.Add(5, Permission.DWDW);
            permissions.Add(6, Permission.DWRD);
            permissions.Add(7, Permission.DWRW);

            permissions.Add(8, Permission.RDDD);
            permissions.Add(9, Permission.RDDW);
            permissions.Add(10, Permission.RDRD);
            permissions.Add(11, Permission.RDRW);

            permissions.Add(12, Permission.RWDD);
            permissions.Add(13, Permission.RWDW);
            permissions.Add(14, Permission.RWRD);
            permissions.Add(15, Permission.RWRW);

            permissions.Add(31, Permission.ReadWriteDelete);

            permissionFriendlyNames.Add(Permission.DDDD, "Deny All");
            permissionFriendlyNames.Add(Permission.RDDD, "Read Normal data only");
            permissionFriendlyNames.Add(Permission.RWDD, "Read and Write Normal data");
            permissionFriendlyNames.Add(Permission.RWRD, "Read and Write Normal data, Read Financial data");
            permissionFriendlyNames.Add(Permission.ReadWriteDelete, "Delete data");
            permissionFriendlyNames.Add(Permission.RWRW, "Allow All");
        }

        /// <summary>
        /// Converts DB char representation of permission to internal type
        /// </summary>
        /// <param name="code">Byte code</param>
        /// <returns>Permission enum value</returns>
        public static Permission GetByCode(byte code)
        {
            if (code > 15)
            {
                return Permission.ReadWriteDelete;
            }
            else
            {
                Permission returnValue = Permission.DDDD;
                permissions.TryGetValue(code, out returnValue);
                return returnValue;
            }
        }

        /// <summary>
        /// Returns permission byte code by its internal type
        /// </summary>
        /// <param name="permission">enum item of Permission</param>
        /// <returns>Byte code</returns>
        public static byte GetCode(this Permission permission)
        {
            return permissions.Single(p => p.Value == permission).Key;
        }

        /// <summary>
        /// Parses code or username
        /// </summary>
        /// <param name="value">code or username</param>
        public static Permission Parse(string value)
        {
            Permission result;
            if (TryParse(value, out result))
                return result;
            return Permission.Unknown;
        }

        /// <summary>
        /// Try to parse code or username
        /// </summary>
        /// <param name="value">code or username</param>
        public static bool TryParse(string value, out Permission permission)
        {
            byte code;
            permission = Permission.Unknown;
            //it might be code
            if (byte.TryParse(value, out code))
            {
                //searh among codes
                if (permissions.ContainsKey(code))
                {
                    permission = permissions[code];
                    return true;
                }
            }

            //or might me user name
            //searh among codes
            if (permissionFriendlyNames.ContainsValue(value))
            {
                permission = permissionFriendlyNames.Where(kvp => kvp.Value == value).Select(kvp => kvp.Key).First();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns all permissions user names 
        /// </summary>
        /// <returns>string names</returns>
        public static IEnumerable<string> GetFriendlyNames()
        {
            return permissionFriendlyNames.Values;
        }

        /// <summary>
        /// Addition of two permissions
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result</returns>
        public static Permission Or(params Permission[] permissions)
        {
            byte current = permissions[0].GetCode();
            if (permissions.Length > 1)
                for (int i = 1; i < permissions.Length; i++)
                {
                    current = (byte) (current | permissions[i].GetCode());
                }
            return GetByCode(current);
        }

        /// <summary>
        /// Multiplication of two permissions 
        /// </summary>
        /// <param name="a">First operand</param>
        /// <param name="b">Second operand</param>
        /// <returns>Result</returns>
        public static Permission And(params Permission[] permissions)
        {
            byte current = permissions[0].GetCode();
            for (int i = 1; i < permissions.Length; i++)
            {
                current = (byte)(current & permissions[i].GetCode());
            }
            return PermissionsProvider.GetByCode(current);
        }

        /// <summary>
        /// Inverts the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        /// <returns>Inverted value</returns>
        public static Permission Not(this Permission permission)
        {
            // 1. XOR: 0101 ^ 11111111 = 11111010
            // 2. SHIFT: 11111010 & 00001111 = 00001010 = 1010
            // 3. Finally: ~0101 = 1010. Formula: (x ^ 255) & 15
            int inverted = (PermissionsProvider.GetCode(permission) ^ 255) & 15;
            return PermissionsProvider.GetByCode((byte)inverted);
        }

        /// <summary>
        /// Returns user name
        /// </summary>
        /// <param name="permission">enum item of Permission</param>
        /// <returns>Byte code</returns>
        public static string GetFriendlyName(this Permission permission)
        {
            return permissionFriendlyNames[permission];
        }

        /// <summary>
        /// Returns if this permission allows reading Normal info.
        /// Shortcut for CompareWithMask method.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool CanRead(this Permission permission, bool isFinancial = false)
        {
            if (isFinancial)
                // 0010                 
                return permission.CompareWithMask(2);
            else
                // 1000            
                return permission.CompareWithMask(8);
        }

        /// <summary>
        /// Returns if this permission allows writing Normal info.
        /// Shortcut for CompareWithMask method.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool CanWrite(this Permission permission, bool isFinancial = false)
        {
            if (isFinancial)
                // 0001                 
                return permission.CompareWithMask(1);
            else
                // 0100                 
                return permission.CompareWithMask(4);
        }

        public static bool CanDelete(this Permission permission)
        {
            return permission.CompareWithMask(Permission.ReadWriteDelete.GetCode());
        }
        
        /// <summary>
        /// Returns if permission satisfies to mask or not.
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="mask">Mask which represents checked byte</param>
        /// <returns></returns>
        public static bool CompareWithMask(this Permission permission, byte mask)
        {
            /*
              Example:
              a) permission: 1010, mask: 1000 ===> 1010 & 1000 = 1000 (result = mask)
              b) permission: 0000, mask: 1000 ===> 0000 & 1000 = 0000 (result != mask)                  
            */
            return (permission.GetCode() & mask) == mask;
        }
    }
}
