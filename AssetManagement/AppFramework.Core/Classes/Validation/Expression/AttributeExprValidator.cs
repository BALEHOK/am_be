using System;
using System.Collections.Generic;
using Antlr.Runtime;

namespace AppFramework.Core.Classes.Validation.Expression
{
    public sealed class AttributeExprValidator : ExprValidator
    {
        public AssetAttribute Attribute { get; private set; }
        private readonly List<AttributeValidationRule> _rules;

        public AttributeExprValidator(string expression, 
            AssetAttribute attribute, 
            List<AttributeValidationRule> rules)
            : base(expression)
        {
            _rules = rules;
            Attribute = attribute;
        }

        protected override ExprParserParser GetParser(ITokenStream input, string value)
        {
            return new AttributeExprParser(input, _rules)
            {                
                Value = value,
                Attribute = Attribute
            };
        }       
    }
}
