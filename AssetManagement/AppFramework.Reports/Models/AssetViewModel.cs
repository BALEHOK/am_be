using System.Collections.Generic;

namespace AppFramework.Reports.Models
{
    public class AssetViewModel
    {
        public string Name { get; set; }
        public List<AssetAttributeViewModel> Attributes { get; set; }
        public List<AssetViewModel> ChildAssets { get; set; }

        public AssetViewModel()
        {
            Attributes = new List<AssetAttributeViewModel>();
            ChildAssets = new List<AssetViewModel>();
        }
    }

    public class AssetAttributeViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
