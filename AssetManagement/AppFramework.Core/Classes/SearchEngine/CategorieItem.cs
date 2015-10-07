namespace AppFramework.Core.Classes.SearchEngine
{
    using System;

    [Serializable]
    public struct CategorieItem
    {
        public int Count { get; set; }
        public string TaxonomyItemName { get; set; }
        public long TaxonomyItemId { get; set; }
    }
}
