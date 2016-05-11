using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models
{
    public class AssetPanelModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<AttributeModel> Attributes { get; set; }
        public bool IsChildAssets { get; set; }
        public long ChildAssetAttrId { get; set; }
    }
}