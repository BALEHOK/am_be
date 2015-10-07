using System;
using System.Linq;
using System.Text;

namespace FormulaBuilder
{    
    public class TokenValue
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public TokenType Type { get; set; }

        public TokenValue ParentValue { get; set; }

        public TokenValue(string typeId)
        {
            Type = TokenType.T(typeId);
            Name = Type.Name;            
        }

        public TokenValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public TokenValue(TokenType type, TokenValue parent = null)
        {
            Type = type;
            Name = type.Name;
            if (parent != null)
                ParentValue = parent;
        }

        public TokenValue SetType(TokenType type)
        {
            Type = type;
            return this;
        }

    }
}