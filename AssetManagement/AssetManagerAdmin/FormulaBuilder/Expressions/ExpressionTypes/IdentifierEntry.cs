namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class IdentifierEntry : ExpressionEntry
    {
        public IdentifierEntry()
        {
            Open = "[";
            Close = "]";
        }

        public override string ToString()
        {
            var name = Selected != null ? Selected.Name : "";
            var type = FindOverride(this) ?? Type;
            var result = string.Format("{0}{1}{2}", type.Open, name, type.Close);
            return result;
        }
    }
}