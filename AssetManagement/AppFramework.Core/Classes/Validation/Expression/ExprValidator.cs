using System;
using System.Collections.Generic;
using System.Linq;

using Antlr.Runtime;
using AppFramework.Core.Validation;


namespace AppFramework.Core.Classes.Validation.Expression
{
    public abstract class ExprValidator
    {
        abstract protected ExprParserParser GetParser(ITokenStream input, string value);

        public string Expression { get; private set; }

        public List<string> ParseErrors
        {
            get
            {
                if (!object.Equals(_parser, null))
                    return _parser.ErrorMessages;
                return new List<string>();
            }
        }

        public List<string> ValidationErrors
        {
            get
            {
                return _parser.ValidationResultLines.Where(l => ! l.IsValid).Select(l => l.Message).ToList();
            }
        }

        private ExprParserParser _parser;

        protected ExprValidator(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException("Expression");
            Expression = expression;
        }

        public ValidationResult Validate(string value)
        {
            var res = new ValidationResult();
            var ex = Expression + Environment.NewLine;
            var lexer = new ExprParserLexer(new ANTLRStringStream(ex));
            var cts = new CommonTokenStream(lexer);
            _parser = GetParser(cts, value);
            _parser.Init();
            _parser.prog();            
            res += _parser.ValidationResultLines.ToArray();
            return res;
        }
    }
}