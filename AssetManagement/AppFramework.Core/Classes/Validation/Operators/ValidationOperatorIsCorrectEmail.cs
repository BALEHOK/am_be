/*--------------------------------------------------------
* ValidationOperatorIsCorrectEmail.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 9/22/2009 10:36:26 AM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    class ValidationOperatorIsCorrectEmail : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            string email = value;
            var res = new ValidationResultLine(string.Empty);
            if (!string.IsNullOrEmpty(email))
            {
                bool valid = Regex.Match(
                    email, 
                    @"^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,3}){1,2}$"
                ).Success;
                res.IsValid = valid;
                if (!valid)
                {
                    res.Message = "Given Email is not valid";
                }
            }
            else
            {
                res.IsValid = true;
            }
            return res;
        }
    }
}
