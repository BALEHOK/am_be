using System.Collections.Generic;
using System.Linq;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public class ExpressionsGrammar
    {
        private readonly List<ExpressionEntry> _data = new List<ExpressionEntry>();
        private string _currentGroup;

        public List<ExpressionEntry> Data { get { return _data; } }

        public List<ExpressionEntry> Get<T>(string name = null)
        {
            var result = name == null ? _data.Where(e => e is T) : _data.Where(e => e is T).Where(e => e.Name == name);
            return result.ToList();
        }

        public FunctionEntry AddFunction(string caption, string name)
        {
            var entry = new FunctionEntry
            {
                DisplayName = caption,
                Name = name,
                Group = _currentGroup,
            };

            _data.Add(entry);
            return entry;
        }

        public ExpressionEntry AddBinaryOperator(string name)
        {
            var entry = new BinaryOperatorEntry
            {
                DisplayName = name,
                Name = name,
                Group = _currentGroup,
            };

            entry.AddLeftOperand(new ExpressionEntry());
            entry.AddRightOperand(new ExpressionEntry());

            _data.Add(entry);
            return entry;
        }

        public ExpressionsGrammar Group(string name)
        {
            _currentGroup = name;
            return this;
        }

        public ExpressionEntry AddCustomType(ExpressionEntry type)
        {
            type.Group = _currentGroup;            
            _data.Add(type);
            return type;
        }
    }
}