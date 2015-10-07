using System.Text.RegularExpressions;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    public class ValidationOperatorIsUrl : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            ValidationResultLine result = new ValidationResultLine(string.Empty);
            result.IsValid = true;
            if (!string.IsNullOrEmpty(value))
            {
                string val = value.ToString();
                string exp = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
                Match m = Regex.Match(val, exp);
                result.IsValid = m.Success;
                if (!result.IsValid)
                {
                    result.Message = string.Format("{0} is not valid Url", val);
                }
            }
            return result;
        }
    }
}
