using System.Data.SqlClient;
namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    public class SearchTerm
    {
        public string CommandText { get; set; }
        public SqlParameter Parameter { get; set; }
    }
}