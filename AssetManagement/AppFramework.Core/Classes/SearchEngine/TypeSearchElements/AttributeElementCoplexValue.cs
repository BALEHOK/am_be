using System.Collections.Generic;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine.TypeSearchElements
{
    public class AttributeElementCoplexValue
    {
        public AssetType ReferencedAssetType { get; set; }
        public List<AttributeElement> Elements { get; set; }
    }
}