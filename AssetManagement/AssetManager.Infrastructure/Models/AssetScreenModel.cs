using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models
{
    public class AssetScreenModel
    {
        public IEnumerable<AssetPanelModel> Panels { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public bool IsMobile { get; set; }

        public long Id { get; set; }

        public int LayoutType { get; set; }

        public bool HasFormula { get; internal set; }
    }
}