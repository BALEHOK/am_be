namespace AppFramework.Core.Classes.SearchEngine.Interface
{
    using AppFramework.Core.Classes.SearchEngine.SearchOperators;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

    public interface ISearchOperator
    {
        SearchTerm Generate(string value, string fieldName);
        SearchTerm GenerateForContext(AttributeElement chain);
    }
}
