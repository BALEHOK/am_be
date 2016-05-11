using System;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.Classes
{
    public class ScreenAttrs : List<AssetAttribute>
    {
        public AssetAttribute this[string name]
        {
            get
            {
                return this.SingleOrDefault(
                    a =>
                        string.Equals(a.Configuration.DBTableFieldName.ToLower(), name.ToLower(),
                            StringComparison.OrdinalIgnoreCase));
            }
        }

        public ScreenAttrs(IEnumerable<AssetAttribute> attrs) : base(attrs)
        {
        }
    }
}