using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes
{
    public class AssetComparer : IEqualityComparer<Asset>
    {
        public bool Equals(Asset x, Asset y)
        {
            return x.UID == y.UID &&
                x.GetConfiguration().UID == y.GetConfiguration().UID;
        }

        public int GetHashCode(Asset obj)
        {
            return obj.UID.GetHashCode() ^ obj.GetConfiguration().UID.GetHashCode();
        }
    }
}
