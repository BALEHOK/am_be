using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.ContextSearchElements;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.Presentation;
using AppFramework.Core.Classes.SearchEngine.SearchOperators;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.Helpers;
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
            var searchId = _unitOfWork.GetMaxSearchId() + 1;
            var dataTable = new DataTable();

            List<SqlParameter> parameters;

            var type = _assetTypeRepository.GetByUid(assetTypeUid);
            var searchQuery = _generateTypeSearchQuery(
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
        /// <param name="assetTypeUid"></param>
        /// <param name="elements">Attribute elements converted to search chain</param>
        /// <param name="configsIds"></param>
        /// <param name="taxonomyItemsIds"></param>
        /// <param name="time"></param>
        /// <param name="order"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<IIndexEntity> FindByType(
            long searchId,
            long userId,
            long assetTypeUid,
            List<AttributeElement> elements,
            string configsIds,
            string taxonomyItemsIds,
            TimePeriodForSearch time,
            Entities.Enumerations.SearchOrder order,
            int pageNumber,
            int pageSize)
        {
            List<SqlParameter> parameters;

            var type = _assetTypeRepository.GetByUid(assetTypeUid);
            var searchQuery = _generateTypeSearchQuery(searchId, type, elements, time, out parameters);

            return FindByTypeContext(searchId, userId, searchQuery, parameters, configsIds, taxonomyItemsIds, time,
                order, pageNumber, pageSize);
        }

        public List<IIndexEntity> FindByTypeContext(
            long searchId,
            long userId,
            long? assetTypeUid,
            IEnumerable<AttributeElement> elements,
            string configsIds,
            string taxonomyItemsIds,
            TimePeriodForSearch time,
            Entities.Enumerations.SearchOrder order,
            int pageNumber,
            int pageSize)
        {
            List<SqlParameter> parameters;
            string searchQuery;

            if (assetTypeUid.HasValue)
            {
                var type = _assetTypeRepository.GetByUid(assetTypeUid.Value);
                searchQuery = _generateTypeSearchQuery(searchId, type,
                    elements.ToSearchChains(type, _unitOfWork, _assetsService, _assetTypeRepository).ToList(), time,
                    out parameters);
            }
            else
            {
                searchQuery = _generateContextSearchQuery(searchId,
                    elements.ToSearchChains(new ContextForSearch(_unitOfWork)).ToList(), time,
                    out parameters);
            }

            return FindByTypeContext(searchId, userId, searchQuery, parameters, configsIds, taxonomyItemsIds, time,
                order, pageNumber, pageSize);
        }

        /// <summary>
        /// Type search
        /// </summary>
        private List<IIndexEntity> FindByTypeContext(
            long searchId,
            long userId,
            string searchQuery,
            List<SqlParameter> parameters,
            string configsIds,
            string taxonomyItemsIds,
            TimePeriodForSearch time,
            Entities.Enumerations.SearchOrder order,
            int pageNumber,
            int pageSize)
        {
            var entities = new List<IIndexEntity>();

            _prefillTemporaryTable(searchId, searchQuery, parameters, _unitOfWork);

            using (var rdResults = _unitOfWork.SqlProvider.ExecuteReader(
                "_cust_SearchByTypeContext",
                new[]
                {
                    new SqlParameter("SearchId", searchId),
                    new SqlParameter("UserId", userId),
                    new SqlParameter("ConfigIds",
                        configsIds != null
                            ? string.Join(" ", configsIds.Split(','))
                            : null),
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
        /// <param name="searchQuery"></param>
        /// <param name="parameters"></param>
        /// <param name="unitOfWork"></param>
        private static void _prefillTemporaryTable(long searchId, string searchQuery, List<SqlParameter> parameters,
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

        /// <summary>
        /// Complex search performs when there are complex attributes dynlists, multipleassets) in search chains
        /// </summary>
        /// <returns></returns>
        private static string _generateTypeSearchQuery(long searchId, AssetType at, List<AttributeElement> elements,
            TimePeriodForSearch period, out List<SqlParameter> parameters)
        {
            #region Build select statement with joins

            // retrieve complex chains
            var complexChains = elements.Where(el => el.IsComplex);

            // join appropriate assets tables for multiple assets elements
            var joins = new HashSet<string>();
            foreach (var element in complexChains)
            {
                var joinLine = string.Empty;

                if (element.ElementType == Enumerators.DataType.Assets)
                {
                    throw new NotImplementedException();
                    //joinLine = string.Format(" LEFT JOIN {0} ON {0}.[{1}] = maa.RelatedDynEntityId AND {0}.{2} = 1 ",
                    //                                    element.FieldSql.Split(new char[] { '.' })[0],
                    //                                    AttributeNames.DynEntityId,
                    //                                    AttributeNames.ActiveVersion);
                }
                if (element.ElementType == Enumerators.DataType.DynList ||
                    element.ElementType == Enumerators.DataType.DynLists)
                {
                    joinLine =
                        string.Format(
                            @" LEFT JOIN DynListValue ON DynListValue.DynEntityConfigUid = [{0}].DynEntityConfigUid AND DynListValue.AssetUid = [{0}].DynEntityUid ",
                            at.DBTableName);
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (!joins.Contains(joinLine))
                    joins.Add(joinLine);
            }

            #endregion

            var query = string.Format(@"SELECT {3}, [{0}].DynEntityUid, [{0}].DynEntityConfigUid 
                                           FROM   [{0}] 
                                           {1} 
                                           {2}",
                at.DBTableName,
                string.Join(" ", joins),
                _getWhereStatement(elements, period, at.DBTableName, out parameters),
                searchId);
            return query;
        }

        private static string _generateContextSearchQuery(long searchId, List<AttributeElement> elements,
            TimePeriodForSearch time, out List<SqlParameter> parameters)
        {
            //construct select statment
            var selectStatement = string.Format("SELECT {3}, [{0}], [{1}] FROM [{2}] WHERE ",
                PropertyUtil.GetName<DynEntityContextAttributesValues>(e => e.DynEntityUid),
                PropertyUtil.GetName<DynEntityContextAttributesValues>(e => e.DynEntityConfigUid),
                typeof (DynEntityContextAttributesValues).Name,
                searchId);

            string timeQuery;

            if (time == TimePeriodForSearch.CurrentTime)
            {
                timeQuery = String.Format(" AND {0} = 1",
                    PropertyUtil.GetName<DynEntityContextAttributesValues>(e => e.IsActive));
            }
            else if (time == TimePeriodForSearch.History)
            {
                timeQuery = String.Format(" AND {0} = 0",
                    PropertyUtil.GetName<DynEntityContextAttributesValues>(e => e.IsActive));
            }

            parameters = new List<SqlParameter>();

            var chainQuery = new StringBuilder();
            var i = 0;
            foreach (var chain in elements)
            {
                var t = BaseOperator.GetOperatorByClassName(chain.ServiceMethod).GenerateForContext(chain);

                parameters.Add(t.Parameter);

                chainQuery.Append(chain.StartBrackets);
                chainQuery.Append(selectStatement);
                chainQuery.AppendFormat("( [{2}] = {0} AND {1} )", chain.ContextUID, t.CommandText,
                    PropertyUtil.GetName<DynEntityContextAttributesValues>(e => e.ContextId));
                chainQuery.Append(chain.EndBrackets);

                if (i != elements.Count - 1)
                {
                    chainQuery.Append(chain.AndOr == 0 ? " INTERSECT " : " UNION ");
                }
                i++;
            }


            return chainQuery.ToString();
        }

        /// <summary>
        /// Returns the SQL where statement
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private static string _getWhereStatement(List<AttributeElement> elements, TimePeriodForSearch period,
            string tableName, out List<SqlParameter> parameters, bool skipBrackets = false)
        {
            var query = string.Empty;
            parameters = new List<SqlParameter>();

            if (elements.Count > 0)
            {
                query = " WHERE ";
                query += "( ";

                for (var i = 0; i < elements.Count; i++)
                {
                    var t = elements[i].GetSearchTerm(tableName);

                    switch (elements[i].ElementType)
                    {
                        case Enumerators.DataType.CurrentDate:
                        case Enumerators.DataType.DateTime:
                            DateTime time;
                            DateTime.TryParse(elements[i].Value, out time);
                            t.Parameter.Value = time;
                            break;
                        case Enumerators.DataType.Int:
                            int intvalue;
                            int.TryParse(elements[i].Value, out intvalue);
                            t.Parameter.Value = intvalue;
                            break;
                        case Enumerators.DataType.Long:
                            long longvalue;
                            long.TryParse(elements[i].Value, out longvalue);
                            t.Parameter.Value = longvalue;
                            break;
                        case Enumerators.DataType.Float:
                            float fvalue;
                            float.TryParse(elements[i].Value, out fvalue);
                            t.Parameter.Value = fvalue;
                            break;
                        case Enumerators.DataType.Bool:
                            var bValue = false;
                            if (elements[i].Value.ToLower() == "true" || elements[i].Value == "1")
                            {
                                bValue = true;
                            }
                            else
                            {
                                bool.TryParse(elements[i].Value, out bValue);
                            }
                            t.Parameter.Value = bValue;
                            break;
                        case Enumerators.DataType.Money:
                        case Enumerators.DataType.Euro:
                            decimal dvalue;
                            decimal.TryParse(elements[i].Value, out dvalue);
                            t.Parameter.Value = dvalue;
                            break;
                    }

                    parameters.Add(t.Parameter);
                    query += skipBrackets ? string.Empty : elements[i].StartBrackets;
                    query += t.CommandText;
                    query += skipBrackets ? string.Empty : elements[i].EndBrackets;
                    if (i < elements.Count - 1)
                    {
                        // it's not last element and it's dynlist(s) and next element is also dynlist => use only OR concatenation
                        if (ChainIsDynamicList(elements[i]) && ChainIsDynamicList(elements[i + 1]))
                        {
                            query += " OR ";
                        }
                        else
                        {
                            query += elements[i].AndOr == 0 ? " AND " : " OR ";
                        }
                    }
                }

                query += ") ";
            }

            switch (period)
            {
                case TimePeriodForSearch.CurrentTime:
                    query += elements.Count > 0 ? " And " : " WHERE ";
                    query += string.Format(" [{0}].[{1}] = 1", tableName, AttributeNames.ActiveVersion);
                    break;
                case TimePeriodForSearch.History:
                    query += elements.Count > 0 ? " And " : " WHERE ";
                    query += string.Format(" [{0}].[{1}] = 0", tableName, AttributeNames.ActiveVersion);
                    break;
                default:
                    break;
            }

            return query;
        }

        /// <summary>
        /// Returns if chain is Dymanic List(s) or not
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ChainIsDynamicList(AttributeElement element)
        {
            return element.ElementType == Enumerators.DataType.DynList ||
                   element.ElementType == Enumerators.DataType.DynLists;
        }
    }
}