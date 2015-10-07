
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    public class ValidationOperatorEqual : ValidationOperatorBase
    { 
        public override ValidationResultLine Validate(string value)
        {
            ValidationResultLine result = new ValidationResultLine(string.Empty)
            {
                IsValid = true,
                Message = "OK"
            };

            int a = int.Parse(value);
            int b = int.Parse(Operands[0].Value.ToString());

            if (a != b)
            {
                result.IsValid = false;
                result.Message = string.Format("{0} not equal to {1}", a, b);
            }

            return result;
        }
    }
}
