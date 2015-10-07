using System.Collections.Generic;

namespace AppFramework.Core.Classes
{
    public interface ITaxonomyItemService
    {
        /// <summary>
        /// Returns TaxonomyItem by its unique ID and active=1
        /// </summary>
        /// <param name="uid">Unique ID</param>
        /// <returns>TaxonomyItem</returns>
        TaxonomyItem GetActiveItemById(long id);

        /// <summary>
        /// Returns TaxonomyItem by its unique ID
        /// </summary>
        /// <param name="uid">Unique ID</param>
        /// <returns>TaxonomyItem</returns>
        TaxonomyItem GetByUid(long uid);

        /// <summary>
        /// Saves taxonomy item and all associated asset types
        /// </summary>
        void Save(Entities.TaxonomyItem taxonomyItem);

        TaxonomyItem GetParentItem(Entities.TaxonomyItem taxonomyItem);

        /// <summary>
        /// Returns the list of all assets assigned to this taxonomy item
        /// </summary>
        /// <returns>List of AssetType</returns>
        List<AssetType> GetAssignedAssetTypes(Entities.TaxonomyItem taxonomyItem);
    }
}