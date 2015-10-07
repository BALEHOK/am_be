using System.Collections.Generic;

namespace AppFramework.Core.Classes
{
    public interface ITaxonomyService
    {
        Taxonomy GetByUid(long uid);

        void Save(Entities.Taxonomy taxonomy, long currentUserId);

        /// <summary>
        /// Returns major edited version of taxonomy by given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Taxonomy GetLastById(long id);

        /// <summary>
        /// Gets the active version of taxonomy
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        Taxonomy GetById(long id);

        /// <summary>
        /// Gets all taxonomies / categories in system
        /// </summary>      
        /// <returns></returns>
        IEnumerable<Taxonomy> GetAll();

        /// <summary>
        /// Deletes the current taxonomy by making it inactive
        /// </summary>
        void Delete(Entities.Taxonomy taxonomy);

        void SetCategoryByUid(long uid);

        /// <summary>
        /// Gets the taxonomy marked as Category
        /// </summary>
        /// <returns>Category</returns>
        Taxonomy GetCategory();
    }
}