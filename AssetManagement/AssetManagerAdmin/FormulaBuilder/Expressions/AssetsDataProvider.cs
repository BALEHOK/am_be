using System.Collections.Generic;
using System.Linq;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public class AssetsDataProvider : IAssetsDataProvider
    {
        public List<AssetTypeModel> AssetTypes { get; set; }
        public AssetTypeModel CurrentAssetType { get; set; }
        public AttributeTypeModel CurrentAttributeType { get; set; }
        public ExpressionEntry CurrentEntry { get; set; }

        private static ExpressionEntry FindContext<T>(ExpressionEntry entry)
        {
            var context = entry.RightOperandsList.SingleOrDefault(e => e is T);
            if (context == null && entry.Parent != null)
                context = FindContext<T>(entry.Parent);
            return context;
        }

        public List<ExpressionEntry> GetItemsFor<T>(T entry) where T : ExpressionEntry
        {
            List<ExpressionEntry> items = null;

            if (typeof(T) == typeof(RelatedFieldValueEntry))
            {
                if (CurrentAssetType == null)
                    return items;

                items = CurrentAssetType.Attributes.OrderBy(a => a.DisplayOrder)
                    .Where(a => a.RelationType != null)
                    .Select(a =>
                    {
                        var item = new RelatedFieldValueEntry
                        {
                            DisplayName = a.DisplayName,
                            Name = a.DbName,
                            SubItems =
                                a.RelationType.Attributes.OrderBy(attr => attr.DisplayOrder).Select(attr =>
                                {
                                    var subitem = new AssetFieldNameEntry
                                    {
                                        DisplayName = attr.DisplayName,
                                        Name = attr.DbName,
                                        Type = new AssetFieldNameEntry()
                                    };

                                    return subitem;
                                }).Cast<ExpressionEntry>().ToList()
                        };
                        return item;
                    })
                    .Cast<ExpressionEntry>()
                    .ToList();
            }
            else if (typeof(T) == typeof(AssetFieldValueEntry))
            {
                if (CurrentAssetType == null)
                    return items;

                items = CurrentAssetType
                    .Attributes
                    .OrderBy(a => a.DisplayOrder)
                    .Select(a =>
                    {
                        var item = new AssetFieldValueEntry
                        {
                            DisplayName = a.DisplayName,
                            Name = a.DbName,
                        };

                        return item;
                    })
                    .Cast<ExpressionEntry>()
                    .ToList();
            }
            else if (typeof(T) == typeof(AssetTypeNameEntry))
            {
                items = AssetTypes.OrderBy(t => t.DisplayName).Select(t =>
                {
                    var item = new AssetTypeNameEntry
                    {
                        DisplayName = t.DisplayName,
                        Name = t.DbName,
                    };
                    return item;
                }).Cast<ExpressionEntry>().ToList();
            }
            else if (typeof(T) == typeof(AssetFieldNameEntry))
            {
                var context = FindContext<AssetTypeNameEntry>(CurrentEntry);
                entry.Context = context;
                if (context != null && context.Selected != null)
                {
                    items = AssetTypes.Single(t => t.DbName == context.Selected.Name)
                        .Attributes.OrderBy(a => a.DisplayOrder).Select(a =>
                        {
                            var item = new AssetFieldValueEntry
                            {
                                DisplayName = a.DisplayName,
                                Name = a.DbName,
                            };
                            return item;
                        }).Cast<ExpressionEntry>().ToList();
                }
            }
            else if (typeof(T) == typeof(NamedVariableEntry))
            {
                var variables = new List<NamedVariableEntry>
                {
                    new NamedVariableEntry {DisplayName = "Current User ID", Name = "CurrentUserId"}
                };
                items = variables.Cast<ExpressionEntry>().ToList();
            }
            else if (typeof(T) == typeof(ValidationFieldValueEntry))
            {
                if (CurrentAssetType != null && CurrentAttributeType != null)
                {
                    items = CurrentAssetType.Attributes.OrderBy(a => a.DisplayOrder).Select(a =>
                    {
                        var item = new AssetFieldValueEntry
                        {
                            DisplayName = a.DisplayName,
                            Name = a.DbName,
                        };

                        return item;
                    }).Cast<ExpressionEntry>().ToList();

                    entry.Items = items;
                    entry.Value = CurrentAttributeType.DbName;
                }
            }
            return items;
        }
    }
}