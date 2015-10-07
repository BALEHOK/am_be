namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class ValidationFieldValueEntry : AssetFieldValueEntry
    {
        public ValidationFieldValueEntry()
        {
            IsEditable = false;
            Type = this;
        }

        public override string ToString()
        {
            return "[@value]";
        }
    }
}