using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    public class ValidationOperatorIsDigit : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            ValidationResultLine result = new ValidationResultLine(string.Empty)
            {
                IsValid = true,
                Message = "OK"
            };

            string exp = @"^\d*$";
            Match m = Regex.Match(value, exp);

            result.IsValid = m.Success;
            if (!result.IsValid)
            {
                result.Message = string.Format("{0} is not a number", value);
            }

            return result;
        }
    }
}
