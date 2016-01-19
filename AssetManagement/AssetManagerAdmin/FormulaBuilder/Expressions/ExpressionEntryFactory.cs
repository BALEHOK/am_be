using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;
using FormulaBuilder.Annotations;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public class ExpressionEntryFactory
    {
        private readonly ExpressionsGrammar _grammar;        
        private readonly ExpressionBuilder _builder;
        private readonly IEntryFactoryDataProvider _dataProvider;

        public ExpressionEntryFactory(ExpressionBuilder builder, ExpressionsGrammar grammar, IEntryFactoryDataProvider dataProvider)
        {
            _builder = builder;
            _grammar = grammar;
            _dataProvider = dataProvider;
        }

        public ExpressionEntry GetIdentifier(ExpressionEntry expressionType, string name, ExpressionEntry currentEntry = null)
        {
            var result = Get<AssetFieldValueEntry>(name, expressionType, currentEntry);

            if (result.Selected == null)
                result = Get<AssetTypeNameEntry>(name, expressionType, currentEntry);
            if (result.Selected == null)
                result = Get<AssetFieldNameEntry>(name, expressionType, currentEntry);

            return result;
        }

        public ExpressionEntry GetByName(ExpressionEntry type, string name, ExpressionEntry currentEntry)
        {
            var method = typeof(ExpressionEntryFactory).GetMethod("Get");
            var genericMethod = method.MakeGenericMethod(type.GetType());
            var entry = (ExpressionEntry)genericMethod.Invoke(this, new object[] { name, type, currentEntry });

            return entry;
        }

        public ExpressionEntry GetByType(ExpressionEntry type, ExpressionEntry currentEntry)
        {
            var method = typeof(ExpressionEntryFactory).GetMethod("Get");
            var genericMethod = method.MakeGenericMethod(type.GetType());
            var entry = (ExpressionEntry)genericMethod.Invoke(this, new object[] { type.Name, type, currentEntry });

            return entry;
        }

        public ExpressionEntry Get<T>([NotNull] string name, ExpressionEntry type, ExpressionEntry currentEntry) where T : ExpressionEntry, new()
        {
            var expressionType = type ?? _grammar.Get<T>(name).FirstOrDefault();

            // this is the main point for creating ExpressionEntry instances
            var entry = new T
            {
                Type = expressionType,
                Builder = _builder
            };

            entry.Type = entry.FindOverride(entry.Type) ?? entry.Type;

            List<ExpressionEntry> items;

            if (typeof(T) == typeof(FunctionEntry))
            {
                items = _grammar.Get<FunctionEntry>().Select(e => e.Clone() as ExpressionEntry).ToList();
            }
            else if (typeof (T) == typeof (BinaryOperatorEntry))
            {
                items = _grammar.Get<BinaryOperatorEntry>().Select(e => e.Clone() as ExpressionEntry).ToList();
            }
            else
            {
                if (_dataProvider == null)
                    throw new Exception("Data Provider can not be NULL");

                _dataProvider.CurrentEntry = currentEntry;
                items = _dataProvider.GetItemsFor(entry);
            }

            if (entry.Type == null)
            {
                entry.Type = _grammar.Get<T>().Single();
            }

            Debug.Assert(entry.Type != null, "Type is null!");

            if (items != null)
            {
                items.ForEach(i => i.Type = entry.Type);
                entry.Items = items;
            }

            if (entry.Value == null)
                entry.Value = name;

            return entry;
        }
    }
}