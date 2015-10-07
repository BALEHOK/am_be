namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    public class MoreEqualOperator : BaseOperator
    {
        private const string operatorValue = ">=";

        public override string GetOperator()
        {
            return operatorValue;
        }
    }
}
