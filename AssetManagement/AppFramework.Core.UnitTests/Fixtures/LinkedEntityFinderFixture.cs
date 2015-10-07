using System;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE.Adapters;

namespace AppFramework.Core.UnitTests.Fixtures
{
    class LinkedEntityFinderFixture : ILinkedEntityFinder
    {
        public long FindRelatedAssetId(AssetType assetType, long assetTypeAttributeId, string value)
        {
            var result = 0;
            if (value == "48000185526")
                result = 345;
            return result;
        }

        public long FindAssetInIndex(long atId, string nodeValue)
        {
            throw new NotImplementedException();
        }

        public string GetRelatedAssetDisplayName(long assetTypeId, long assetTypeAttributeId, long assetId, bool activeVersion)
        {
            return "48000185526";
        }
    }
}
