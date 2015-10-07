using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes.SearchEngine.Interface;
using AppFramework.Core.Classes.SearchEngine.Enumerations;

namespace AppFramework.Core.Classes.SearchEngine
{
    public class SearchCondition : ISearchCondition
    {
        public string FieldName { get; set; }

        public string Value { get; set; }

        public ConcatenationOperation AndOr { get; set; }

        public Enumerations.SearchOperator SearchOperator { get; set; }

        public SearchCondition()
        {
        }

        public SearchCondition(string fieldName, string value,
            ConcatenationOperation andOr = ConcatenationOperation.And, SearchOperator searchOperator = Enumerations.SearchOperator.Equal)
        {
            FieldName = fieldName;
            Value = value;
            AndOr = andOr;
            SearchOperator = searchOperator;
        }
    }
}
