using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;
using NCalc;
using NCalc.Domain;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public class ExpressionParser
    {
        private readonly ExpressionEntryFactory _expressionEntryFactory;
        private readonly Stack<ExpressionEntry> _stack = new Stack<ExpressionEntry>();
        private readonly ExpressionBuilder _builder;
        private ExpressionEntry _rootEntry;
        private ExpressionEntry _currentExpression;

        public ExpressionParser(IEntryFactoryDataProvider dataProvider, ExpressionBuilder builder, ExpressionsGrammar grammar)
        {
            _builder = builder;
            _expressionEntryFactory = new ExpressionEntryFactory(builder, grammar, dataProvider);
        }

        public ExpressionEntry Parse(string input)
        {
            var expressionString = string.Empty;
            if (input != null)
            {
                expressionString = input.Trim();
            }

            _rootEntry = null;
            _currentExpression = null;
            _stack.Clear();

            if (!string.IsNullOrEmpty(expressionString))
            {
                var expression = Expression.Compile(expressionString, false);
                var visitor = new ExpressionVisitor();
                visitor.OnNewEntry += (sender, exp) => ConnectExpression(exp);
                expression.Accept(visitor);
            }
            
            _builder.SetRootEntry(_rootEntry);
            return _rootEntry;
        }

        private ExpressionEntry FindCurrentExpression(Stack<ExpressionEntry> stack)
        {
            var top = stack.Peek();
            var filledNumber = top.AllOperands.Count;
            var operandsNumber = top.Type.AllOperands.Count;

            if (operandsNumber == filledNumber)
            {
                if (_stack.Count > 1)
                {
                    _stack.Pop();
                    FindCurrentExpression(_stack);
                }
            }

            return _stack.Peek();
        }

        private void ConnectExpression(LogicalExpression expression)
        {
            var entry = ConvertExpression(expression);

            if (_rootEntry == null)
            {
                _rootEntry = entry;
                _currentExpression = entry;
            }

            var currentEntry = _stack.Count > 0 ? _stack.Peek() : null;

            if (expression is BinaryExpression || expression is Function || expression is UnaryExpression)
            {
                _stack.Push(entry);
            }

            if (currentEntry == null)
                return;

            if (currentEntry is BinaryOperatorEntry)
            {
                if (currentEntry.LeftOperandsList.Count == 0)
                {
                    currentEntry.AddLeftOperand(entry);
                }
                else if (currentEntry.RightOperandsList.Count == 0)
                {
                    currentEntry.AddRightOperand(entry);
                }
            }
            else if (currentEntry is FunctionEntry)
            {
                var type = currentEntry.Type.RightOperandsList[currentEntry.RightOperandsList.Count];
                entry.Overrides = type.Overrides;
                entry.IsExtendable = type.IsExtendable;

                currentEntry.AddRightOperand(entry);
            }

            _currentExpression = FindCurrentExpression(_stack);
        }

        private ExpressionEntry ConvertExpression(LogicalExpression expression)
        {
            var binary = expression as BinaryExpression;
            if (binary != null)
                return ConvertExpression(binary);

            var function = expression as Function;
            if (function != null)
                return ConvertExpression(function);

            var identifier = expression as Identifier;
            if (identifier != null)
                return ConvertExpression(identifier);

            var value = expression as ValueExpression;
            if (value != null)
                return ConvertExpression(value);

            return null;
        }

        private string ConvertBinaryExpressionType(BinaryExpressionType type)
        {
            switch (type)
            {
                case BinaryExpressionType.Plus:
                    return "+";
                case BinaryExpressionType.Minus:
                    return "-";
                case BinaryExpressionType.Times:
                    return "*";
                case BinaryExpressionType.Div:
                    return "/";
                case BinaryExpressionType.And:
                    return "and";
                case BinaryExpressionType.Or:
                    return "or";
                case BinaryExpressionType.Lesser:
                    return "<";
                case BinaryExpressionType.LesserOrEqual:
                    return "<=";
                case BinaryExpressionType.Greater:
                    return ">";
                case BinaryExpressionType.GreaterOrEqual:
                    return ">=";
                case BinaryExpressionType.Equal:
                    return "=";
                case BinaryExpressionType.NotEqual:
                    return "<>";

                default:
                    throw new ArgumentException("unknown binary operator type", "type");
            }
        }

        private ExpressionEntry ConvertExpression(BinaryExpression expression)
        {
            var name = ConvertBinaryExpressionType(expression.Type);
            var entry = _expressionEntryFactory.Get<BinaryOperatorEntry>(name, null, _currentExpression);
            return entry;
        }

        private ExpressionEntry ConvertExpression(Function function)
        {
            var entry = _expressionEntryFactory.Get<FunctionEntry>(function.Identifier.Name, null, _currentExpression);
            return entry;
        }

        private ExpressionEntry ConvertExpression(Identifier identifier)
        {
            var identifierName = identifier.Name.Trim('$', '@', '#');

            ExpressionEntry entry = null;

            if (identifier.Name.Equals("@value"))
            {                
                entry = _expressionEntryFactory.Get<ValidationFieldValueEntry>(identifierName, null, _currentExpression);
            }
            else if (identifier.Name.StartsWith("#"))
            {
                entry = _expressionEntryFactory.Get<NamedVariableEntry>(identifierName, null, _currentExpression);
            }
            else if (identifierName.Contains('@'))
            {
                entry = _expressionEntryFactory.Get<RelatedFieldValueEntry>(identifierName, null, _currentExpression);
            }

            if (entry == null)
                entry = _expressionEntryFactory.GetIdentifier(null, identifierName, _currentExpression);

            return entry;
        }

        private ExpressionEntry ConvertExpression(ValueExpression valueExpression)
        {
            var entry = new ValueEntry
            {
                Builder = _builder,
                Value = valueExpression.Value.ToString(),
            };

            return entry;
        }
    }
}