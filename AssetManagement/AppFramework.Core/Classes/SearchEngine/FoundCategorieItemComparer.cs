using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.SearchEngine
{
    class FoundCategorieItemComparer : IEqualityComparer<CategorieItem>
    {
        public bool Equals(CategorieItem x, CategorieItem y)
        {
            return x.TaxonomyItemId == y.TaxonomyItemId;
        }

        public int GetHashCode(CategorieItem obj)
        {
            return obj.TaxonomyItemId.GetHashCode();
        }
    }
}
