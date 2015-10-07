using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using AppFramework.ConstantsEnumerators;
using System.Xml;
using System.Xml.Linq;

namespace AppFramework.Core.Classes.IE.Providers
{
    internal class LDAPProvider
    {
        /// <summary>
        /// Gets the error status of current provider
        /// </summary>
        public StatusInfo Status { get; private set; } 

        private LDAPCredentials _credentials;

        public LDAPProvider(LDAPCredentials credentials) 
        {            
            _credentials = credentials;
            Status = new StatusInfo();
        }

        public SearchResultCollection GetUsers(List<string> propertiesToLoad)
        {   
            DirectoryEntry baseEntry 
                = new DirectoryEntry("LDAP://" + _credentials.Domain, 
                                                _credentials.UserName, 
                                                _credentials.Password, 
                                                AuthenticationTypes.Secure);

            DirectorySearcher search = new DirectorySearcher(baseEntry);
            SearchResultCollection resultCol = null; ;

            search.PropertiesToLoad.AddRange(propertiesToLoad.ToArray());            
            search.Filter = "(&(objectClass=user)(objectCategory=person))";
            search.PageSize = 1000;

            try
            {
                resultCol = search.FindAll();

                if (resultCol == null)
                    throw new System.Exception("Cannot retrieve the AD users");
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                baseEntry.Close();
            }           
            
            return resultCol;            
        }

        /// <summary>
        /// Method tries to establish the connection with AD 
        /// and returns status of that operation.
        /// </summary>
        /// <returns></returns>
        public bool CheckConnection()
        {
            bool result = false;
            DirectoryEntry baseEntry = new DirectoryEntry("LDAP://" + _credentials.Domain, 
                _credentials.UserName, _credentials.Password, AuthenticationTypes.Secure);
           
            try
            {
                object o = baseEntry.NativeObject;
                result = true;
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                baseEntry.Close();
            }
            return result;
        }
    }
}
