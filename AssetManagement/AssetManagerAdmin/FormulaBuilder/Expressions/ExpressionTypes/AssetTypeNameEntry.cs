namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class AssetTypeNameEntry : IdentifierEntry
    {
        public AssetTypeNameEntry()
        {
            Open = "[$";
            DisplayName = "Type Name";
            IsLocalContext = true;
        }
    }
}