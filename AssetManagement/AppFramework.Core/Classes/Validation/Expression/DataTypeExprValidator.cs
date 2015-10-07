using System.Collections.Generic;
using AppFramework.Core.DataTypes;

namespace AppFramework.Core.Classes.Validation.Expression
{
    public sealed class DataTypeExprValidator : ExprValidator
    {
        public DataTypeBase DataType { get; private set; }
        private readonly List<DataTypeValidationRule> _validationRules;

        public DataTypeExprValidator(string expression, 
            DataTypeBase dataType, 
            List<DataTypeValidationRule> validationRules)
            : base(expression)
        {
            DataType = dataType;
            _validationRules = validationRules;
        }

        protected override ExprParserParser GetParser(Antlr.Runtime.ITokenStream input, string value)
        {
            return new DataTypeExprParser(input, DataType, _validationRules)
            {
                Value = value
            };
        }
    }
}
