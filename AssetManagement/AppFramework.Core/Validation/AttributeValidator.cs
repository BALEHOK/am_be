using System;
using System.Diagnostics;
using System.Web;
using AppFramework.Core.Classes;
using AppFramework.Core.Helpers;

namespace AppFramework.Core.Validation
{
    public class AttributeValidator
    {
        private readonly ValidationFunctions _validationFunctions;

        public AttributeValidator(ValidationFunctions validationFunctions)
        {
            _validationFunctions = validationFunctions;
        }

        private string EscapeStringParameters(string expression)
        {
            expression.AllIndexesOf("['").ForEach(i =>
            {
                var end = expression.IndexOf("']", i, StringComparison.Ordinal);
                var start = i + 2;
                var length = end - start;
                var escapeString = expression.Substring(start, length);

                escapeString = HttpUtility.UrlEncode(escapeString);

                start--;
                expression = expression.Remove(start, length + 2);
                Debug.Assert(escapeString != null, "escapeString != null");
                expression = expression.Insert(start, escapeString);
            });

            return expression;
        }

        public ValidationResult Validate(AssetAttribute attribute)
        {
            var value = attribute.Data.Value;

            var result = ValidationResult.Success;

            var validationExpression = attribute.Configuration.ValidationExpr;

            //todo: migrate existing expressions
            if (validationExpression != null && validationExpression.StartsWith("@"))
                validationExpression = null;

            if (string.IsNullOrWhiteSpace(validationExpression))
                return result;

            var encodedExpression = EscapeStringParameters(validationExpression);
            var evaluator = new ValidationExpressionEvaluator(encodedExpression, _validationFunctions, result);

            evaluator.Expression.EvaluateParameter += (name, args) =>
            {
                args.Result = name.Equals("@value")
                    ? TypesHelper.GetTypedValue(attribute.Configuration.Base.DataType.FrameworkDataType, value)
                    : HttpUtility.UrlDecode(name);
            };

            try
            {
                var isValid = evaluator.Evaluate();

                if (!isValid)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var displayExpr = validationExpression.Replace("@value", attribute.Configuration.Name);
                    result +=
                        ValidationResultLine.Error(
                            attribute.Configuration.ID.ToString(),
                            string.Format("Validation failed: {0}",
                                displayExpr));
                }
                else
                {
                    // clean all errors if final result is true
                    result.ResultLines.Clear();
                }
            }
            catch (Exception e)
            {
                result += ValidationResultLine.Error(attribute.Configuration.ID.ToString(), e.Message);
            }

            return result;
        }
    }
}