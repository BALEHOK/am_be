using System;
using Antlr.Runtime;
using IList = System.Collections.IList;
using ArrayList = System.Collections.ArrayList;
using Stack = Antlr.Runtime.Collections.StackList;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Expression
{
    abstract public partial class ExprParserParser : Parser
    {
        protected List<ValidationResultLine> _validationResultLines;

        public List<ValidationResultLine> ValidationResultLines
        {
            get
            {
                return (object.Equals(_validationResultLines, null)) ? 
                    new List<ValidationResultLine>()
                    : _validationResultLines;
            }
        }

        public bool Result
        {
            get
            {
                return _result;
            }
        }

        public List<string> ErrorMessages
        {
            get
            {
                return _errorMessages;
            }
        }

        public bool IsErrors
        {
            get
            {
                return _errorMessages.Count > 0;
            }
        }

        public string Value
        {
            set;
            get;
        }

        public void Init()
        {            
            _validationResultLines = new List<ValidationResultLine>();
            _result = true;
        }

        abstract public bool EvaluteByAlias(string alias);        
    }
}