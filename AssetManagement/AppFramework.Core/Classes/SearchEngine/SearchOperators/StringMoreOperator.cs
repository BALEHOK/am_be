using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    public class StringMoreOperator : BaseOperator
    {
        private const string operatorValue = ">";

        public override string GetOperator()
        {
            return operatorValue;
        }
    }
}
