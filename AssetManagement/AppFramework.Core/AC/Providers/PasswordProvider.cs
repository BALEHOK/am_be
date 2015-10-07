/*--------------------------------------------------------
* PasswordProvider.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 9/3/2009 12:44:07 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace AppFramework.Core.AC.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Security.Cryptography;
    using AppFramework.Core.Classes;

    public class PasswordProvider
    {
        public PasswordProvider()
        {
        }

        public string Encrypt(string clearPassword)
        {
            string encodedPassword = clearPassword;                        
            var sha1 = SHA1.Create();
            encodedPassword = Convert.ToBase64String(sha1.ComputeHash(Encoding.Unicode.GetBytes(clearPassword)));
            return encodedPassword;
        }
    }
}
