/*--------------------------------------------------------
* ValidationOperatorIsDate.cs
* 
* Copyright: DAXX
* Author: aNesterov
* Created: 11/10/2009 9:52:33 AM
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
    using System.Threading;
    using System.Globalization;

    public class ValidationOperatorIsDatetime : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            var result = new ValidationResultLine(string.Empty);
            DateTime dt = DateTime.Now;
            if (string.IsNullOrEmpty(value))
                result.IsValid = true;
            else
                result.IsValid = DateTime.TryParse(value, ApplicationSettings.DisplayCultureInfo.DateTimeFormat, DateTimeStyles.None, out dt);
            return result;
        }
    }
}
