using System.Text.RegularExpressions;
using AppFramework.Core.Validation;


namespace AppFramework.Core.Classes.Validation.Operators
{
    class ValidationOperatorReqEx : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            ValidationResultLine result = new ValidationResultLine(string.Empty)
            {
                IsValid = true,
                Message = "OK"
            };

            string val = value;
            string exp = Operands[0].Value.ToString();

            Match m = Regex.Match(val, exp);

            result.IsValid = m.Success;
            if (!result.IsValid)
            {
                result.Message = string.Format("{0} is not match with {1} pattern", val, exp);
            }

            return result;
        }
    }
}
