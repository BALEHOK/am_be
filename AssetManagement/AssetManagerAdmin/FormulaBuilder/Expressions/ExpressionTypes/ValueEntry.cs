using System;
using AssetManagerAdmin.FormulaBuilder.Controls;
using AssetManagerAdmin.FormulaBuilder.ValueEditors;

namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class ValueEntry : IdentifierEntry
    {
        public ValueEntry()
        {
            EditorType = typeof (TextValueEditor);
            DisplayName = "Value";
            Type = this;
        }

        public override string ToString()
        {
            Decimal number;
            return Decimal.TryParse(Value, out number) ? Value : string.Format("\'{0}\'", Value);
        }
    }
}