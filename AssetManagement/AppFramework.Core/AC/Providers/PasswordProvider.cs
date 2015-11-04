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

using System;
using System.Security.Cryptography;
using System.Text;

namespace AppFramework.Core.AC.Providers
{
    public class PasswordProvider
    {
        public string Encrypt(string clearPassword)
        {
            var sha1 = SHA1.Create();
            return Convert.ToBase64String(sha1.ComputeHash(Encoding.Unicode.GetBytes(clearPassword)));
        }
    }
}