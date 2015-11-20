namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    /// <summary>
    /// Operator is intent to be used in conditions with related asset
    /// Should be "&lt;>" when used in simple condition
    /// </summary>
    public class AssetNotInOperator : BaseOperator
    {
        private const string operatorValue = "<>";

        public override string GetOperator()
        {
            return operatorValue;
        }
    }
}