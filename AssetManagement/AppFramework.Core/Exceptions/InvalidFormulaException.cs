using System;
namespace AppFramework.Core.Exceptions
{
    public class InvalidFormulaException : Exception
    {
        public InvalidFormulaException(string formula, string message)
            : base(string.Format("Invalid formula {0}, {1}", formula, message))
        {

        }

        public InvalidFormulaException(string formula, Exception ex)
            : base(string.Format("Invalid formula {0}", formula), ex)
        {

        }        
    }
}
