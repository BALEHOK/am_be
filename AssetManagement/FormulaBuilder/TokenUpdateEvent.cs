using System;

namespace FormulaBuilder
{
    public class TokenUpdateEvent : EventArgs
    {
        public TokenType Token { get; private set; }
        public string Value { get; private set; }
        public TokenUpdateEvent(TokenType token, string value = null)
        {
            Token = token;
            Value = value;
        }
    }
}