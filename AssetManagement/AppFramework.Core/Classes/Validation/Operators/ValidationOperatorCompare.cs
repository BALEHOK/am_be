using System;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    public class ValidationOperatorCompare : ValidationOperatorBase
    {        
        #region IValidationOperator Members
        public override ValidationResultLine Validate(string value)
        {
            ValidationResultLine result = new ValidationResultLine(string.Empty)
            {
                IsValid = true,
                Message = "OK"
            };
                        
            long val = 0;
            if (long.TryParse(value, out val))
            {
                string CompareOp = string.Empty;
                long CompareVal = 0;

                if (Operands.Count == 2)
                {
                    CompareOp = Operands[0].Value.ToString();
                    CompareVal = long.Parse(Operands[1].Value.ToString());
                }

                switch (CompareOp)
                {
                    case ">":
                        result.IsValid = val > CompareVal;
                        break;
                    case "<":
                        result.IsValid = val < CompareVal;
                        break;
                    case "=":
                        result.IsValid = val == CompareVal;
                        break;
                    default:
                        throw new Exception("CompareOp must be <, >, =");
                }

                if (!result.IsValid)
                {
                    result.Message = string.Format("{0} not {1} {2}", val, CompareOp, CompareVal);
                }
            }
            else
            {
                result.IsValid = false;
                result.Message = "Only numbers acceptable for compare";
            }           

            return result;
        }
        #endregion
    }
}
