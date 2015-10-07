using System;
using System.Linq;
using NCalc;
using NCalc.Domain;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public class ExpressionVisitor : LogicalExpressionVisitor
    {
        public event EventHandler<LogicalExpression> OnNewEntry;

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
            if (OnNewEntry != null)
                OnNewEntry(this, expression);

            expression.LeftExpression.Accept(this);
            expression.RightExpression.Accept(this);
        }

        public override void Visit(UnaryExpression expression)
        {
            expression.Accept(this);
        }

        public override void Visit(ValueExpression expression)
        {
            // expression as string value
            var stringValue = expression.Value as String;
            if (stringValue != null && stringValue.StartsWith("$"))
            {
                var expressionString = stringValue.Trim('$');
                Expression.Compile(expressionString, false).Accept(this);
            }
            else
            {
                if (OnNewEntry != null)
                    OnNewEntry(this, expression);
            }
        }

        public override void Visit(Function function)
        {
            if (OnNewEntry != null)
                OnNewEntry(this, function);

            function.Expressions.ToList().ForEach(e => e.Accept(this));
        }

        public override void Visit(Identifier expression)
        {
            if (OnNewEntry != null)
                OnNewEntry(this, expression);
        }
    }
}