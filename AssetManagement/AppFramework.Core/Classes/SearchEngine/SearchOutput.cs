using System.Collections.Generic;
using AppFramework.Core.Classes.SearchEngine.Presentation;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine
{
    /// <summary>
    /// TODO: refactor as obsolete
    /// </summary>
    public class SearchOutput
    {
        public List<IIndexEntity> Entities { get; set; }
        public List<FoundAssetType> FoundAssetTypes { get; set; }
        public List<CategorieItem> FoundCategories { get; set; }
    }
}
