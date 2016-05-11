using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace AppFramework.Core.Classes.IE
{
    public class AssetsExporter : IAssetsExporter
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetsService _assetsService;
        private readonly ITypeSearch _typeSearch;

        public AssetsExporter(
            IAssetTypeRepository assetTypeRepository, 
            IUnitOfWork unitOfWork,
            IAssetsService assetsService,
            ITypeSearch typeSearch)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException();
            if (unitOfWork == null)
                throw new ArgumentNullException();
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            _typeSearch = typeSearch;
            _assetTypeRepository = assetTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public DataTable ExportToDataTable(long assetTypeUid, List<AttributeElement> filter, long userId)
        {
            var type = _assetTypeRepository.GetByUid(assetTypeUid);
            var dataTable = _typeSearch.FillAssetToDataTableByTypeContext(userId, assetTypeUid, type.DBTableName, filter);
            var columnsDataTable = GetAssetColumnsByTypeContext(type.DBTableName);

            foreach (DataRow row in columnsDataTable.Rows)
            {
                //retrieving related asset
                int relatedAssetTypeId;
                int.TryParse(row["RelatedAssetTypeID"].ToString(), out relatedAssetTypeId);
                if (relatedAssetTypeId > 0)
                {
                    AssetType assetType;
                    if (!_assetTypeRepository.TryGetById(relatedAssetTypeId, out assetType)) // related asset type was deactivated
                    {
                        continue;
                    }

                    var newColumnName = row["dbtablefieldname"] + "temp";
                    dataTable.Columns.Add(newColumnName, typeof(string));

                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        var columnName = row["dbtablefieldname"].ToString();
                        var cellValue = dataRow[columnName].ToString();
                        if (string.IsNullOrEmpty(cellValue))
                            continue;

                        int entityId;
                            Asset asset;
                        if (int.TryParse(cellValue, out entityId)
                            && entityId > 0
                            && _assetsService.TryGetAssetById(entityId, assetType, out asset))
                        {
                            var attributeValue = asset["Name"].Value;
                            dataRow[newColumnName] = attributeValue;
                        }
                    }

                    dataTable.Columns.Remove(row["dbtablefieldname"].ToString());
                    dataTable.Columns[newColumnName].ColumnName = row["dbtablefieldname"].ToString();
                }
                //if row type is dynlist
                else if (bool.Parse(row["IsDynListValue"].ToString()))
                {
                    //get dyn list uid
                    int dynListUid = int.Parse(row["DynListUid"].ToString());

                    var newColumnName = row["dbtablefieldname"] + "temp";
                    dataTable.Columns.Add(newColumnName, typeof(string));

                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        //get corresponding dyn list value
                        int assetUid = int.Parse(dataRow["DynEntityUid"].ToString());
                        int dynEntityConfigUid = int.Parse(dataRow["DynEntityConfigUid"].ToString());

                        //get string represantation
                        var list = _unitOfWork.DynListValueRepository.Where(item => item.DynListUid == dynListUid && item.AssetUid == assetUid && item.DynEntityConfigUid == dynEntityConfigUid);
                        if (list.Count != 0)
                        {
                            var dynListValue = list[0];
                            var dynListItemUid = dynListValue.DynListItemUid;
                            var valueList = _unitOfWork.DynListItemRepository.Where(item => item.DynListItemUid == dynListItemUid && item.DynListUid == dynListUid);

                            if (valueList.Count != 0)
                            {
                                var dynListItem = valueList[0];
                                //set corresponding value
                                dataRow[newColumnName] = dynListItem.Value;
                            }
                        }
                    }

                    dataTable.Columns.Remove(row["dbtablefieldname"].ToString());
                    dataTable.Columns[newColumnName].ColumnName = row["dbtablefieldname"].ToString();
                }
            }

            var columnsDictionary = new Dictionary<string, string>();

            foreach (DataRow row in columnsDataTable.Rows)
            {
                columnsDictionary.Add(row["dbtablefieldname"].ToString(), row["name"].ToString());
            }

            foreach (DataColumn column in dataTable.Columns)
            {
                if (columnsDictionary.ContainsKey(column.ColumnName) && !dataTable.Columns.Contains(columnsDictionary[column.ColumnName]))
                {
                    column.ColumnName = columnsDictionary[column.ColumnName];
                }
            }
            return dataTable;
        }

        private DataTable GetAssetColumnsByTypeContext(string tableName)
        {
            var dataTable = new DataTable();
            using (var connection = new EntityConnection("name=DataEntities"))
            using (var unitOfWork = new UnitOfWork(connection))
            {
                var reader = unitOfWork.SqlProvider.ExecuteReader(
                    "_cust_GetColumnNamesByTypeContext",
                    new SqlParameter[]
                        {
                            new SqlParameter("TableName", tableName)
                        },
                    CommandType.StoredProcedure);

                dataTable.Load(reader);
            }
            return dataTable;
        }
    }
}
