using System;
using System.Collections.Generic;
using Antlr.Runtime;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using System.Linq;

namespace AppFramework.Core.Classes.Validation.Expression
{
    public sealed class DataTypeExprParser : ExprParserParser
    {
        public DataTypeBase DataType { get; private set; }
        private readonly List<DataTypeValidationRule> _validationRules;

        public DataTypeExprParser(ITokenStream input, 
            DataTypeBase dataType, 
            List<DataTypeValidationRule> validationRules)
            : base(input)
        {
            if (dataType == null)
                throw new ArgumentNullException("DataType");
            if (validationRules == null)
                throw new ArgumentNullException("List<DataTypeValidationRule>");
            DataType = dataType;
            _validationRules = validationRules;
        }

        public override bool EvaluteByAlias(string alias)
        {
            bool result = true;
            string al = alias.TrimStart('@');
            var r = _validationRules.FirstOrDefault(rule => rule.Name == al);
            if (!object.Equals(r, null))
            {
                var ln = r.Validate(Value);
                result = ln.IsValid;
                if (!ln.IsValid)
                {
                    _validationResultLines.Add(ln);
                }
            }
            else
            {
                result = false;
                ErrorMessages.Add(string.Format("{0} rule not found", al));
            }
            return result;
        }
    }
}
