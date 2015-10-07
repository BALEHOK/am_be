using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    public class ValidationOperatorIsIP : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            ValidationResultLine result = new ValidationResultLine(string.Empty)
            {
                IsValid = true,
                Message = "OK"
            };

            Match m = Regex.Match(value, @"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");
            result.IsValid = m.Success;
            if (!result.IsValid)
            {
                result.Message = string.Format("{0} is not a IP address", value);
            }

            return result;
        }              
    }
}
