namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class NamedVariableEntry : IdentifierEntry
    {
        public NamedVariableEntry()
        {
            Open = "[#";
            DisplayName = "Variable";
        }
    }
}