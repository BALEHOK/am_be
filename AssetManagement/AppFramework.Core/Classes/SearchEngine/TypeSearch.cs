using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.Presentation;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes.SearchEngine
{
    /// <summary>
    /// Contains the logic for search by AssetType
    /// </summary>
    public class TypeSearch : ITypeSearch
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;

        public TypeSearch(
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
        }

        /// <summary>
        /// Returns datatable with asset by asset type and search parameters
        /// </summary>
        /// <param name="userId">current user id</param>
        /// <param name="assetTypeUid">asset type uid</param>
        /// <param name="tableName">table name</param>
        /// <param name="elements">search parameters</param>
        /// <returns></returns>
        public DataTable FillAssetToDataTableByTypeContext(
            long userId,
            long assetTypeUid,
            string tableName,
            IEnumerable<AttributeElement> elements)
        {
            var searchId = Guid.NewGuid();
            var dataTable = new DataTable();

            List<SqlParameter> parameters;

            var type = _assetTypeRepository.GetByUid(assetTypeUid);
            var searchQuery = SearchQueryBuilder.GenerateTypeSearchQuery(
                searchId,
                type,
                elements.ToSearchChains(type, _unitOfWork, _assetsService, _assetTypeRepository).ToList(),
                TimePeriodForSearch.CurrentTime,
                out parameters);

            _prefillTemporaryTable(searchId, searchQuery, parameters, _unitOfWork);
            var reader = _unitOfWork.SqlProvider.ExecuteReader(
                "_cust_GetExportDataByTypeContext",
                new[]
                {
                    new SqlParameter("SearchId", searchId),
                    new SqlParameter("TableName", tableName),
                    new SqlParameter("UserId", userId)
                },
                CommandType.StoredProcedure);

            dataTable.Load(reader);

            return dataTable;
        }

        /// <summary>
        /// Advance search by type. Overload for v2
        /// </summary>
        /// <param name="searchId"></param>
        /// <param name="userId"></param>
        /// <param name="assetTypeId"></param>
        /// <param name="elements">Attribute elements converted to search chain</param>
        /// <param name="taxonomyItemsIds"></param>
        /// <param name="time"></param>
        /// <param name="order"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<IIndexEntity> FindByType(Guid searchId, long userId, long assetTypeId,
            List<AttributeElement> elements, string taxonomyItemsIds, TimePeriodForSearch time,
            Entities.Enumerations.SearchOrder order, int pageNumber, int pageSize)
        {
            List<SqlParameter> parameters;

            var type = _assetTypeRepository.GetById(assetTypeId);
            if (type == null)
            {
                return new List<IIndexEntity>(0);
            }

            var searchQuery = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, type, elements, time, out parameters);

            return FindByType(searchId, userId, searchQuery, parameters, taxonomyItemsIds, time,
                order, pageNumber, pageSize);
        }

        /// <summary>
        /// Type search
        /// </summary>
        private List<IIndexEntity> FindByType(Guid searchId, long userId, string searchQuery,
            List<SqlParameter> parameters, string taxonomyItemsIds, TimePeriodForSearch time,
            Entities.Enumerations.SearchOrder order, int pageNumber, int pageSize)
        {
            var entities = new List<IIndexEntity>();

            _prefillTemporaryTable(searchId, searchQuery, parameters, _unitOfWork);

            using (var rdResults = _unitOfWork.SqlProvider.ExecuteReader(
                "_cust_SearchByTypeContext",
                new[]
                {
                    new SqlParameter("SearchId", searchId),
                    new SqlParameter("UserId", userId),
                    new SqlParameter("ConfigIds", null),
                    new SqlParameter("taxonomyItemsIds",
                        taxonomyItemsIds != null
                            ? string.Join(" ", taxonomyItemsIds.Split(','))
                            : null),
                    new SqlParameter("active", time == TimePeriodForSearch.CurrentTime),
                    new SqlParameter("orderby", (byte) order),
                    new SqlParameter("PageNumber", pageNumber),
                    new SqlParameter("PageSize", pageSize)
                },
                CommandType.StoredProcedure))
            {
                var typesInSearchLocalCache = new Dictionary<long, AssetType>();
                var helper = new Helper(_unitOfWork);
                while (rdResults.Read())
                {
                    var entity = new f_cust_SearchByTypeContext_Result
                    {
                        IndexUid = rdResults.GetInt64(0),
                        DynEntityUid = rdResults.GetInt64(1)
                    };
                    if (rdResults[2] != DBNull.Value)
                        entity.BarCode = rdResults.GetString(2);
                    if (rdResults[3] != DBNull.Value)
                        entity.Name = rdResults.GetString(3);
                    if (rdResults[4] != DBNull.Value)
                        entity.Description = rdResults.GetString(4);
                    if (rdResults[5] != DBNull.Value)
                        entity.Keywords = rdResults.GetString(5);
                    if (rdResults[6] != DBNull.Value)
                        entity.EntityConfigKeywords = rdResults.GetString(6);
                    if (rdResults[7] != DBNull.Value)
                        entity.AllAttrib2IndexValues = rdResults.GetString(7);
                    if (rdResults[8] != DBNull.Value)
                        entity.AllContextAttribValues = rdResults.GetString(8);
                    if (rdResults[9] != DBNull.Value)
                        entity.AllAttribValues = rdResults.GetString(9);
                    if (rdResults[10] != DBNull.Value)
                        entity.CategoryKeywords = rdResults.GetString(10);
                    if (rdResults[11] != DBNull.Value)
                        entity.TaxonomyKeywords = rdResults.GetString(11);
                    if (rdResults[12] != DBNull.Value)
                        entity.User = rdResults.GetString(12);
                    if (rdResults[13] != DBNull.Value)
                        entity.LocationUid = rdResults.GetInt64(13);
                    if (rdResults[14] != DBNull.Value)
                        entity.Location = rdResults.GetString(14);
                    if (rdResults[15] != DBNull.Value)
                        entity.Department = rdResults.GetString(15);
                    if (rdResults[16] != DBNull.Value)
                        entity.DynEntityConfigUid = rdResults.GetInt64(16);
                    if (rdResults[17] != DBNull.Value)
                        entity.UpdateDate = rdResults.GetDateTime(17);
                    if (rdResults[18] != DBNull.Value)
                        entity.CategoryUids = rdResults.GetString(18);
                    if (rdResults[19] != DBNull.Value)
                        entity.TaxonomyUids = rdResults.GetString(19);
                    if (rdResults[20] != DBNull.Value)
                        entity.OwnerId = rdResults.GetInt64(20);
                    if (rdResults[21] != DBNull.Value)
                        entity.DepartmentId = rdResults.GetInt64(21);
                    if (rdResults[22] != DBNull.Value)
                        entity.DynEntityId = rdResults.GetInt64(22);
                    if (rdResults[23] != DBNull.Value)
                        entity.TaxonomyItemsIds = rdResults.GetString(23);
                    if (rdResults[24] != DBNull.Value)
                        entity.DynEntityConfigId = rdResults.GetInt64(24);
                    if (rdResults[25] != DBNull.Value)
                        entity.DisplayValues = rdResults.GetString(25);
                    if (rdResults[26] != DBNull.Value)
                        entity.DisplayExtValues = rdResults.GetString(26);
                    if (rdResults[27] != DBNull.Value)
                        entity.UserId = rdResults.GetInt64(27);
                    entity.rownumber = rdResults.GetInt32(28);

                    if (!typesInSearchLocalCache.ContainsKey(entity.DynEntityConfigId))
                        typesInSearchLocalCache.Add(entity.DynEntityConfigId,
                            _assetTypeRepository.GetById(entity.DynEntityConfigId));

                    entity.Subtitle = helper.BuildSubtitle(entity,
                        typesInSearchLocalCache[entity.DynEntityConfigId]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        /// <summary>
        /// Because we cannot pass dynamic set of parameters to the SP, we have to pre-fill the temporary table
        /// </summary>
        private static void _prefillTemporaryTable(Guid searchId, string searchQuery, List<SqlParameter> parameters,
            IUnitOfWork unitOfWork)
        {
            var parametersDefinition = string.Join(", ", from p in parameters
                select string.Format("{0} {1}", p.ParameterName,
                    p.SqlDbType == SqlDbType.NVarChar || p.SqlDbType == SqlDbType.VarChar
                        ? string.Format("{0}({1})", p.SqlDbType, p.Size)
                        : p.SqlDbType.ToString()));

            unitOfWork.SqlProvider.ExecuteNonQuery(
                string.Format("DELETE FROM {0} WHERE SearchId=@SearchId;", Constants.TypeContextSearchTempTable),
                new[] {new SqlParameter("@SearchId", searchId)},
                CommandType.Text, false);
            var query = string.Format("INSERT {0} EXECUTE sp_executesql N'{1}', N'{2}' {3};",
                Constants.TypeContextSearchTempTable,
                searchQuery,
                parametersDefinition,
                (parameters.Count > 0)
                    ? ", " + string.Join(", ", from p in parameters
                        select string.Format("{0}={0}", p.ParameterName))
                    : string.Empty
                );
            unitOfWork.SqlProvider.ExecuteNonQuery(query, parameters.ToArray(), CommandType.Text, false);
        }
    }
}