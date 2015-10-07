using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes.SearchEngine.Enumerations;

namespace AppFramework.Core.Classes.SearchEngine.Interface
{
    public interface ISearchCondition
    {
        string FieldName { get; set; }
        string Value { get; set; }
        ConcatenationOperation AndOr { get; set; }
        SearchOperator SearchOperator { get; set; }
    }
}
