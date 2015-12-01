using System.Collections.Generic;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public interface IAssetsDataProvider : IEntryFactoryDataProvider
    {
        List<AssetTypeModel> AssetTypes { get; set; }
        AssetTypeModel CurrentAssetType { get; set; }
        AttributeTypeModel CurrentAttributeType { get; set; }
    }
}