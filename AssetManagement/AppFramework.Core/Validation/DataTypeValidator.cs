using System;
using AppFramework.Core.Classes;
using AppFramework.Core.Helpers;

namespace AppFramework.Core.Validation
{
    public class DataTypeValidator
    {
        private readonly ValidationFunctions _validationFunctions;

        public DataTypeValidator(ValidationFunctions validationFunctions)
        {
            _validationFunctions = validationFunctions;
        }

        public ValidationResult Validate(AssetAttribute attribute)
        {
            var attributeName = attribute.Configuration.ID.ToString();
            var dataType = attribute.Data.DataType;
            var value = attribute.Data.Value;

            var result = ValidationResult.Success;

            if (value == null)
                return result;

            var isValidType = true;
            try
            {
                TypesHelper.GetTypedValue(dataType.FrameworkDataType, value);
            }
            catch (FormatException)
            {
                isValidType = false;
            }
            catch (InvalidCastException)
            {
                isValidType = false;
            }
            catch (OverflowException)
            {
                result += ValidationResultLine.Error(attributeName, "Value exceeds maximum for this type");
                return result;
            }

            var dataTypeValidationExpression = dataType.Base.ValidationExpr;
            if (!string.IsNullOrEmpty(dataTypeValidationExpression))
            {
                if (dataTypeValidationExpression.StartsWith("@"))
                {
                    result +=
                        ValidationResultLine.Error(
                            attributeName,
                            string.Format("Validation expression for '{0}' data type should be updated",
                                dataType.Name));
                    return result;
                }

                var evaluator = new ValidationExpressionEvaluator(dataTypeValidationExpression, _validationFunctions,
                    result);
                evaluator.Expression.EvaluateParameter += (name, args) =>
                {
                    if (name.Equals("@value"))
                        args.Result = TypesHelper.GetTypedValue(dataType.FrameworkDataType, value);
                };
                if (!evaluator.Evaluate())
                    result += ValidationResultLine.Error(attributeName, dataType.Base.ValidationMessage);
            }

            if (!isValidType)
            {
                result +=
                    ValidationResultLine.Error(
                        attributeName,
                        string.Format("Validation failed: value '{0}' is not valid ", value));
            }

            return result;
        }
    }
}