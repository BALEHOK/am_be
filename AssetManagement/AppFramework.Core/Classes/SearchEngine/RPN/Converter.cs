using System;

namespace AppFramework.Core.Classes.SearchEngine.RPN
{
    /// <summary>
    /// Utilizes conversion of operation to Reverse Polish Notation (RPN)
    /// </summary>
    public class Converter
    {
        protected Opus6.Stack _stack;

        protected int GetPriority(char c)
        {
            switch (c)
            {
                case '(': return 0;
                case ')': return 1;
                case '-':
                case '+': return 2;
                case '*':
                case '/': return 3;
            }
            return -1;
        }

        public Converter()
        {
            _stack = new Opus6.StackAsLinkedList();
        }

        protected bool IsArithmeticOperation(char c)
        {
            return "-+/*".IndexOf(c) != -1;
        }

        public string ToRpn(string expression)
        {
            // clean stack
            _stack.Purge();
            string output = "";

            // go with each char
            foreach (char c in expression)
            {
                // if closing bracket...
                if (c == ')')
                {
                    // get operations from stack while opening bracket
                    char op = (char)_stack.Pop();
                    while (op != '(')
                    {
                        output += op;
                        op = (char)_stack.Pop();
                    }
                }
                // if digit...
                else if (Char.IsDigit(c))
                {
                    // concat with output string
                    output += c;
                }
                // if opening bracket...
                else if (c == '(')
                {
                    // put it to the stack
                    _stack.Push(c);
                }
                // if operation...
                else if (IsArithmeticOperation(c))
                {
                    // if stack is empty...
                    if (_stack.IsEmpty)
                    {
                        // move operation to the stack
                        _stack.Push(c);
                    }
                    else
                    {
                        // check priorities
                        if (GetPriority(c) > GetPriority((char)_stack.Top))
                        {
                            _stack.Push(c);
                        }
                        else
                        {

                            while (!_stack.IsEmpty &&
                                   GetPriority(c) <= GetPriority((char)_stack.Top))
                            {
                                output += (char)_stack.Pop(); ;
                            }
                            _stack.Push(c);
                        }
                    }
                }
            }

            // concat the rest of symbols to the output string
            while (!_stack.IsEmpty)
            {
                output += (char)_stack.Pop();
            }

            return output;
        }
    }
}
