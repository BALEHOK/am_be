
using System.Data.SqlClient;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    public class LikeOperator : BaseOperator
    {
        private const string operatorValue = "LIKE";

        public override SearchTerm Generate(string value, string fieldName)
        {
            string paramName = string.Format("@parameter{0}", GetHashCode());
            SearchTerm term = new SearchTerm();
            term.CommandText = string.Format("{0} {1} {2}",
                fieldName.Contains("[") ? fieldName : string.Format("[{0}]", fieldName),
                operatorValue,
                paramName);
            term.Parameter = new SqlParameter(paramName, string.Format("%{0}%", value));
            return term;
        }

        public override SearchTerm GenerateForContext(AttributeElement chain)
        {
            string paramName = string.Format("{0}", string.Format("@parameter{0}", GetHashCode()));
            SearchTerm term = new SearchTerm();
            term.Parameter = new SqlParameter(paramName, string.Format("%{0}%", chain.Value));
            string leftPart = " StringValue ";

            if (string.IsNullOrEmpty(term.CommandText))
                term.CommandText = string.Format(" {0} {1} {2} ", leftPart, GetOperator(), paramName);

            return term;
        }

        public override string GetOperator()
        {
            return operatorValue;
        }
    }
}
