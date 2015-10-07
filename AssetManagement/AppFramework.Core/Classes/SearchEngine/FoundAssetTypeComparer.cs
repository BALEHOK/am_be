using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes.SearchEngine.Presentation;

namespace AppFramework.Core.Classes.SearchEngine
{
    class FoundAssetTypeComparer : IEqualityComparer<FoundAssetType>
    {
        public bool Equals(FoundAssetType x, FoundAssetType y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(FoundAssetType obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
