using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    
    public class ValidationOperatorIsFloat : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            ValidationResultLine result = new ValidationResultLine(string.Empty)
            {
                IsValid = true,
                Message = "OK"
            };

            float fres = 0;

            // trying to parse value as float using current culture settings, if value is null result is True
            // to avoid null values use Required flag for attribute
            result.IsValid = string.IsNullOrEmpty(value) || float.TryParse(value, System.Globalization.NumberStyles.Float, ApplicationSettings.DisplayCultureInfo.NumberFormat, out fres);

            if (!result.IsValid)
            {
                result.Message = string.Format("{0} is not a float", value);
            }

            return result;
        }
    }
}
