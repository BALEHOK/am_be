using System.Collections.Generic;
using System.Data;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AppFramework.Core.Classes.SearchEngine
{
    public interface ITypeSearch
    {
        /// <summary>
        /// Returns datatable with asset by asset type and search parameters
        /// </summary>
        /// <param name="userId">current user id</param>
        /// <param name="assetTypeUid">asset type uid</param>
        /// <param name="tableName">table name</param>
        /// <param name="elements">search parameters</param>
        /// <returns></returns>
        DataTable FillAssetToDataTableByTypeContext(
            long userId,
            long assetTypeUid,
            string tableName,
            IEnumerable<AttributeElement> elements);

        /// <summary>
        /// Type search
        /// </summary>
        List<Entities.IIndexEntity> FindByTypeContext(
            long searchId,
            long userId,
            long? assetTypeUid,
            IEnumerable<AttributeElement> elements,
            string configsIds,
            string taxonomyItemsIds,
            TimePeriodForSearch time,
            Entities.Enumerations.SearchOrder order,
            int pageNumber,
            int pageSize);
    }
}