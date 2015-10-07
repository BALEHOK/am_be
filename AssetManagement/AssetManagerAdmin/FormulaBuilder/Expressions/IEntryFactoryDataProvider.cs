using System.Collections.Generic;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public interface IEntryFactoryDataProvider
    {
        ExpressionEntry CurrentEntry { get; set; }
        List<ExpressionEntry> GetItemsFor<T>(T entry) where T : ExpressionEntry;
    }
}