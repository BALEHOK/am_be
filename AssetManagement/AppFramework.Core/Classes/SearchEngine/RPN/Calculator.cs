using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.SearchEngine.RPN
{
    /// <summary>
    /// Utilizes Reverse Polish Notation (RPN)
    /// </summary>
    public class Calculator
    {
        protected Opus6.Stack _stack;
        protected Converter _converter;

        public Calculator()
        {
            _stack = new Opus6.StackAsLinkedList();
            _converter = new Converter();
        }

        public double Calculate(string expression)
        {
            _stack.Purge();
            foreach (char c in _converter.ToRpn(expression))
            {
                if (Char.IsDigit(c))
                {
                    _stack.Push(Convert.ToInt32(Char.GetNumericValue(c)));
                }
                else
                {
                    int op1 = (int)_stack.Pop();
                    int op2 = (int)_stack.Pop();
                    _stack.Push(DoOperation(Convert.ToInt32(op2), Convert.ToInt32(op1), c));
                }
            }

            return (int)_stack.Pop();
        }

        protected int DoOperation(int op1, int op2, int operation)
        {
            switch (operation)
            {
                case '+': return op1 + op2;
                case '-': return op1 - op2;
                case '*': return op1 * op2;
                case '/': return op1 / op2;
            }
            return 0;
        }
    }
}
