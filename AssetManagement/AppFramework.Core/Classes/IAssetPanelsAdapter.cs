using AppFramework.Entities;
using System.Collections.Generic;

namespace AppFramework.Core.Classes
{
    public interface IAssetPanelsAdapter
    {
        IDictionary<AssetAttribute, Asset> DependencyDescriptor { get; }

        Dictionary<AttributePanel, List<AssetAttribute>> GetPanelsByScreen(AssetWrapperForScreenView assetWrapper, AssetTypeScreen screen);

        Dictionary<AttributePanel, List<AssetAttribute>> GetDefaultPanels(Asset asset);
    }
}
