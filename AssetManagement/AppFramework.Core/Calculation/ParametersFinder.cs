using System.Collections.Generic;
using System.Linq;
using NCalc.Domain;

namespace AppFramework.Core.Calculation
{
    /// <summary>
    /// NCalc visitor implementation. Finds parameters names in expression
    /// </summary>
    public class ParametersFinder : LogicalExpressionVisitor
    {
        readonly List<string> _parameters = new List<string>();

        public IEnumerable<string> Parameters
        {
            get { return _parameters; }
        }

        public override void Visit(LogicalExpression expression)
        {            
            expression.Accept(this);
        }

        public override void Visit(TernaryExpression expression)
        {
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
            expression.MiddleExpression.Accept(this);
        }

        public override void Visit(BinaryExpression expression)
        {
            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(UnaryExpression expression)
        {
            expression.Accept(this);
        }

        public override void Visit(ValueExpression expression)
        {
        }

        public override void Visit(Function function)
        {
            function.Expressions.ToList().ForEach(e => e.Accept(this));
        }

        public override void Visit(Identifier function)
        {
            if (!_parameters.Contains(function.Name))
                _parameters.Add(function.Name);
        }
    }
}