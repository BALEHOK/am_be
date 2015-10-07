using System;
using AppFramework.Core.Calculation;
using NCalc;

namespace AppFramework.Core.Validation
{
    public class ValidationExpressionEvaluator
    {
        private readonly Expression _expression;
        private readonly FunctionsFactory<bool, ValidationResult> _functionsFactory;
        private ValidationResult _validationResult;

        public Expression Expression { get { return _expression; } }

        public ValidationExpressionEvaluator(string expressionText,
            FunctionsFactory<bool, ValidationResult> functionsFactory, ValidationResult validationResult = null)
        {            
            _expression = new Expression(expressionText);
            _functionsFactory = functionsFactory;
            _validationResult = validationResult ?? ValidationResult.Success;

            _expression.EvaluateFunction += (name, args) =>
            {
                var function = _functionsFactory.EvaluateFunction(name, validationResult, args);
                args.Result = function;
            };
        }

        public bool Evaluate()
        {
            try
            {
                return (bool)_expression.Evaluate();
            }
            catch (Exception e)
            {
                _validationResult += ValidationResultLine.Error(string.Empty, e.Message);
                return false;
            }
        }
    }
}