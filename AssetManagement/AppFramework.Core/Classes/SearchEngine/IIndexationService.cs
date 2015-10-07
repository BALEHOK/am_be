namespace AppFramework.Core.Classes.SearchEngine
{
    public interface IIndexationService
    {
        /// <summary>
        /// Adds asset to fast indexing system (for future using in stock scanner and inventarisation module)
        /// </summary>
        /// <param name="asset"></param>
        void UpdateIndex(Asset asset);
    }
}