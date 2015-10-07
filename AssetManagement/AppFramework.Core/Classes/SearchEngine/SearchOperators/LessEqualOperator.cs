namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    public class LessEqualOperator : BaseOperator
    {
        private const string operatorValue = "<=";

        public override string GetOperator()
        {
            return operatorValue;
        }
    }
}
