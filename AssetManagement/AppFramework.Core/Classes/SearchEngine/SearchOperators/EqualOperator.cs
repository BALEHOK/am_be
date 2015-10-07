using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.Interface;

namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    class EqualOperator : BaseOperator
    {
        private const string operatorValue = "=";

        public override string GetOperator()
        {
            return operatorValue;
        }
    }
}
