using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Expression
{
    public sealed class AttributeExprParser : ExprParserParser
    {
        private readonly List<AttributeValidationRule> _rules;

        public AttributeExprParser(ITokenStream input, List<AttributeValidationRule> rules)
            : base(input)
        {
            if (rules == null)
                throw new ArgumentNullException("rules");
            _rules = rules;
        }

        public AssetAttribute Attribute { get; set; }

        public override bool EvaluteByAlias(string alias)
        {
            bool result = true;
            var validationRule = _rules.FirstOrDefault(rule => rule.Name == alias.TrimStart('@'));

            if (!object.Equals(validationRule, null))
            {
                validationRule.ValidationOperator.Attribute = Attribute;
                ValidationResultLine ln = validationRule.Validate(Value);
                result = ln.IsValid;
                if (!ln.IsValid)
                {
                    ValidationResultLines.Add(ln);
                }
            }
            else
            {
                result = false;
                ValidationResultLines.Add(new ValidationResultLine(string.Empty)
                    {
                        IsValid = false,
                        Message = string.Format("{0} rule not found", alias)
                    });
            }
            return result;
        }
    }
}
