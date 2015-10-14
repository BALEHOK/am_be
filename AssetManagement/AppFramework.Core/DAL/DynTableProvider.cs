
using System.Collections;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using Common.Logging;
using Dapper;
using LinqKit;
using Opus6;

namespace AppFramework.Core.DAL
{
    using Adapters;
    using AppFramework.ConstantsEnumerators;
    using Classes;
    using ConstantsEnumerators;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class DynTableProvider : ITableProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDynColumnAdapter _dynColumnAdapter;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public DynTableProvider(IUnitOfWork unitOfWork, IDynColumnAdapter dynColumnAdapter)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            if (dynColumnAdapter == null)
                throw new ArgumentNullException("IDynColumnAdapter");
            _unitOfWork = unitOfWork;
            _dynColumnAdapter = dynColumnAdapter;
        }

        /// <summary>
        /// Returns data record of asset
        /// </summary>
        /// <param name="_assetTypeUID">AssetType unique ID</param>
        /// <param name="_assetUID">Asset unique ID</param>
        /// <returns>DynRow object</returns>
        public DynRow GetRowByUid(Entities.DynEntityConfig assetConfig, long assetUid)
        {
            return GetRow(assetConfig, new SqlParameter[] {
                    new SqlParameter(){ ParameterName = AttributeNames.DynEntityUid, Value = assetUid, DbType= System.Data.DbType.Int64 }
                });
        }

        /// <summary>
        /// Returns data record of active asset
        /// </summary>
        /// <param name="_assetTypeUID">AssetType unique ID</param>
        /// <param name="_assetUID">Asset unique ID</param>
        /// <returns>DynRow object</returns>
        public DynRow GetRowById(Entities.DynEntityConfig assetConfig, long assetId)
        {
            return GetRow(assetConfig, new SqlParameter[] {
                    new SqlParameter(){ ParameterName = AttributeNames.ActiveVersion, Value = true, DbType= System.Data.DbType.Boolean }, 
                    new SqlParameter(){ ParameterName = AttributeNames.DynEntityId, Value = assetId, DbType= System.Data.DbType.Int64 }, 
                });
        }

        public DynRow GetRowByIdAndRevision(Entities.DynEntityConfig assetConfig, long assetId, int revision)
        {
            return GetRow(
                assetConfig, 
                new SqlParameter[] 
                {
                    new SqlParameter
                    { 
                        ParameterName = AttributeNames.Revision, 
                        Value = revision, 
                        DbType= System.Data.DbType.Int32 
                    }, 
                    new SqlParameter
                    { 
                        ParameterName = AttributeNames.DynEntityId, 
                        Value = assetId, 
                        DbType= System.Data.DbType.Int64 
                    }, 
                });
        }

        /// <summary>
        /// Returns the count of assets by given AssetType
        /// </summary>
        /// <param name="assetType">AssetType</param>
        /// <param name="options">Field-Value pairs for WHERE clause</param>
        /// <returns>Number of assets</returns>
        public static int GetCount(Entities.DynEntityConfig assetConfig, Dictionary<string, string> options)
        {
            if (assetConfig == null)
                throw new NullReferenceException("AssetType was not provided");

            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(*) AS Count FROM ");
            sb.Append(assetConfig.DBTableName);
            sb.Append(" WHERE ");
            string whereString = string.Join(" AND ", from pair in options
                                                      select string.Format("{0}=@{1}", pair.Key, pair.Key));
            sb.Append(whereString);
            int count = 0;
            var unitOfWork = new DataProxy.UnitOfWork();
            var result = unitOfWork.SqlProvider.ExecuteScalar(sb.ToString(), (from item in options
                                                                              select new SqlParameter()
                                                                              {
                                                                                  ParameterName = item.Key,
                                                                                  Value = item.Value
                                                                              }).ToArray());
            int.TryParse(result.ToString(), out count);
            return count;
        }

        /// <summary>
        /// Returns list of data records of given AssetType
        /// </summary>
        /// <param name="_assetTypeID">unique ID of AssetType</param>
        /// <param name="onlyActiveVersion">Only active revisions will be retrieved</param>
        /// <returns>List of DynRow objects</returns>
        public List<DynRow> GetAllRowsByAssetConfiguration(Entities.DynEntityConfig assetConfig, bool IsActiveVersion, long? rowStart = null, long? rowsNumber = null)
        {
            var options = new Dictionary<string, string>(1);
            if (IsActiveVersion)
                options.Add(AttributeNames.ActiveVersion, bool.TrueString);
            return GetRows(assetConfig, options, rowStart, rowsNumber);
        }

        /// <summary>
        /// Returns the list of data records for all inactive revisions of given asset
        /// by it's ID
        /// </summary>
        /// <param name="assetTypeUID">UID of AssetType</param>
        /// <param name="assetId">ID of asset</param>
        /// <param name="rowsNumber">Number of rows to retrieve</param>
        /// <returns>List of DynRow objects</returns>
        public List<DynRow> GetHistoryAssets(Entities.DynEntityConfig assetConfig, long assetId, long? rowStart, long? rowsNumber)
        {
            var options = new Dictionary<string, string>(2)
            {
                {AttributeNames.DynEntityId, assetId.ToString()},
                {AttributeNames.ActiveVersion, bool.FalseString}
            };
            return GetRows(assetConfig, options, rowStart, rowsNumber, AttributeNames.Revision);
        }

        /// <summary>
        /// Returns the row of an asset data by given asset configuration 
        /// and additional options
        /// </summary>
        /// <param name="assetConfig">AssetType object</param>
        /// <param name="options">Collection of options</param>
        /// <returns>DynRow object with asset data</returns>
        DynRow GetRow(Entities.DynEntityConfig assetConfig, SqlParameter[] parameters)
        {
            if (assetConfig == null)
                throw new NullReferenceException("AssetType was not provided");

            var tableRow = new DynRow { TableName = assetConfig.DBTableName };

            tableRow.Fields.AddRange(from attr in assetConfig.DynEntityAttribConfigs
                                     select _dynColumnAdapter.ConvertDynEntityAttribConfigToDynColumn(attr));

            var query = new StringBuilder();
            query.AppendFormat("SELECT * FROM [{0}] ", tableRow.TableName);
            if (parameters != null && parameters.Any())
            {
                query.Append(" WHERE ");
                string whereString = string.Join(" AND ", from par in parameters
                                                          select string.Format("[{0}]=@{1}", par.ParameterName, par.ParameterName));
                query.Append(whereString);
            }

            using (var reader = _unitOfWork.SqlProvider.ExecuteReader(query.ToString(), parameters))
            {
                if (reader.Read())
                {
                    foreach (DynColumn field in tableRow.Fields)
                    {
                        _assignValue(reader, field);
                    }
                }
                else
                {
                    tableRow = null;
                }
                reader.Close();
            }
            return tableRow;
        }


        /// <summary>
        /// Returns first the row of an asset data by given asset configuration 
        /// and additional options
        /// </summary>
        /// <param name="assetConfig">AssetType object</param>
        /// <returns>DynRow object with asset data</returns>
        public DynRow GetFirstActiveRow(Entities.DynEntityConfig assetConfig)
        {
            if (assetConfig == null)
                throw new NullReferenceException("AssetType was not provided");

            DynRow tableRow = new DynRow();
            tableRow.TableName = assetConfig.DBTableName;

            tableRow.Fields.AddRange(from attr in assetConfig.DynEntityAttribConfigs
                                     where attr.Active
                                     select _dynColumnAdapter.ConvertDynEntityAttribConfigToDynColumn(attr));

            var query = new StringBuilder();
            query.AppendFormat("SELECT Top(1) * FROM [{0}] where ActiveVersion=1", tableRow.TableName);



            var reader = _unitOfWork.SqlProvider.ExecuteReader(query.ToString());
            if (reader.Read())
            {
                foreach (DynColumn field in tableRow.Fields)
                {
                    _assignValue(reader, field);
                }
            }
            else
            {
                tableRow = null;
            }
            reader.Close();
            return tableRow;
        }

        public List<DynRow> GetRows(Entities.DynEntityConfig assetConfig,
                                           List<SqlParameter> options = null,
                                           long? rowStart = null,
                                           long? rowsNumber = null,
                                           string orderBy = AttributeNames.Name,
                                           List<long> idsList = null)
        {
            if (assetConfig == null)
                throw new NullReferenceException("DynEntityConfig");
            if (!assetConfig.DynEntityAttribConfigs.Any())
                throw new System.ArgumentException("DynEntityAttribConfigs are not loaded");

            var sb = new StringBuilder();
            sb.Append("WITH Assets AS ( ");
            sb.AppendFormat("SELECT *, ROW_NUMBER() OVER (ORDER BY [{0}]) AS RowNumber ", orderBy);
            sb.AppendFormat(" FROM [{0}]  WHERE 1=1 ", assetConfig.DBTableName);

            if (options != null && options.Count > 0)
            {
                foreach (var parameter in options)
                {
                    switch (parameter.DbType)
                    {
                        case DbType.String:
                            sb.Append(" AND " + string.Join(" AND ", string.Format("[{0}] like @{0}", parameter.ParameterName)));
                            break;
                        default:
                            sb.Append(" AND " + string.Join(" AND ", string.Format("[{0}] = @{0}", parameter.ParameterName)));
                            break;
                    }
                }
            }

            if (idsList != null && idsList.Count > 0)
            {
                sb.AppendFormat(" AND {0} IN ({1})", AttributeNames.DynEntityId, string.Join(",", idsList));
            }

            if (!rowStart.HasValue || !rowsNumber.HasValue)
                sb.Append(" ) SELECT * FROM Assets ");
            else
            {
                sb.AppendFormat(" ) SELECT TOP {0} * FROM Assets ", rowsNumber);
                sb.AppendFormat("WHERE RowNumber > {0} ", rowStart);
            }

            sb.AppendFormat(" ORDER BY [{0}];", orderBy);

            var rows = new List<DynRow>(10);
            using (var reader = _unitOfWork.SqlProvider.ExecuteReader(sb.ToString(), options.ToArray()))
            {
                while (reader.Read())
                {
                    var tableRow = new DynRow(assetConfig.DBTableName, (from attr in assetConfig.DynEntityAttribConfigs
                                                                        select _dynColumnAdapter.ConvertDynEntityAttribConfigToDynColumn(attr))
                                                                       .ToList());
                    foreach (var field in tableRow.Fields)
                    {
                        _assignValue(reader, field);
                    }
                    rows.Add(tableRow);
                }
                reader.Close();
            }
            return rows;
        }
        
        // todo: remove this method
        private Dictionary<long, AssetType> GetTypeRevisions(long typeId)
        {
            var includes = new IncludesBuilder<DynEntityConfig>();
            includes.Add(e => e.DynEntityAttribConfigs.Select(a => a.DataType));

            var typeRevisions = _unitOfWork.DynEntityConfigRepository
                .Where(c => c.DynEntityConfigId == typeId, includes.Get())
                .ToDictionary(c => c.DynEntityConfigUid, c => new AssetType(c, _unitOfWork));

            return typeRevisions;
        }

        public List<DynRow> GetRows(long typeId, Dictionary<string, string> options = null, string orderBy = null,
            bool desc = false, Dictionary<long, AssetType> typeRevisions = null)
        {
            var includes = new IncludesBuilder<DynEntityConfig>();
            includes.Add(e => e.DynEntityAttribConfigs.Select(a => a.DataType));
            var allTypes = typeRevisions ?? GetTypeRevisions(typeId);

            var dbTableName = allTypes.First().Value.DBTableName;
            var queryString =
                new StringBuilder(string.Format(@"SELECT * FROM [{0}] WHERE 1=1", dbTableName));

            DynamicParameters queryParameters = null;
            if (options != null && options.Count > 0)
            {
                queryParameters = new DynamicParameters();
                foreach (var pair in options)
                    queryParameters.Add(pair.Key, pair.Value);

                queryString.Append(" AND " +
                                   string.Join(" AND ",
                                       from pair in options select string.Format("[{0}]=@{0}", pair.Key)));
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var order = desc ? "DESC" : "";
                queryString.Append(string.Format(" ORDER BY [{0}] {1}", orderBy, order));
            }

            IEnumerable<dynamic> assets;
            using (var connection = new SqlConnection(_unitOfWork.SqlProvider.ConnectionString))
            {
                var queryResult = connection.Query(queryString.ToString(), queryParameters).ToList();
                assets = queryResult;
            }

            var rows = new List<DynRow>();
            assets.Cast<IEnumerable>().ForEach(e =>
            {
                var attributes = e.Cast<KeyValuePair<string, object>>().ToDictionary(a => a.Key, a => a.Value);
                var typeData = allTypes[(long) attributes["DynEntityConfigUid"]].Base;

                var columns = new List<DynColumn>();
                attributes.ForEach(a =>
                {
                    var attributeConfig =
                        typeData.DynEntityAttribConfigs.SingleOrDefault(attr => attr.DBTableFieldname == a.Key);

                    if (attributeConfig == null)
                        return;

                    var datatype = new CustomDataType(attributeConfig.DataType);
                    var column = new DynColumn(attributeConfig.DBTableFieldname,
                        datatype,
                        !attributeConfig.IsRequired,
                        attributeConfig.Name == AttributeNames.DynEntityUid);

                    if (a.Value != null)
                    {
                        try
                        {
                            column.Value = Convert.ChangeType(a.Value, column.DataType.FrameworkDataType,
                                ApplicationSettings.PersistenceCultureInfo);
                        }
                        catch (FormatException)
                        {
                            column.Value = Convert.ToString(a.Value, ApplicationSettings.PersistenceCultureInfo);
                        }
                    }

                    columns.Add(column);
                });

                var row = new DynRow(dbTableName, columns);
                rows.Add(row);
            });

            return rows;
        }

        /// <summary>
        /// Returns the rows of assets data by given asset configuration 
        /// and additional options
        /// </summary>
        /// <param name="assetType">AssetType object</param>
        /// <param name="options">Collection of options</param>
        /// <param name="whereString">Additional SQL expression</param>
        /// <param name="rowsNumber">Number of rows to retrieve</param>
        /// <returns>List of DynRow objects with assets data</returns>
        public List<DynRow> GetRows(Entities.DynEntityConfig assetConfig,
                                           Dictionary<string, string> options = null,
                                           long? rowStart = null,
                                           long? rowsNumber = null,
                                           string orderBy = AttributeNames.Name,
                                           List<long> idsList = null)
        {
            if (assetConfig == null)
                throw new NullReferenceException("DynEntityConfig");
            if (!assetConfig.DynEntityAttribConfigs.Any())
                throw new System.ArgumentException("DynEntityAttribConfigs are not loaded");

            var sb = new StringBuilder();
            sb.Append("WITH Assets AS ( ");
            sb.AppendFormat("SELECT *, ROW_NUMBER() OVER (ORDER BY [{0}]) AS RowNumber ", orderBy);
            sb.AppendFormat(" FROM [{0}]  WHERE 1=1 ", assetConfig.DBTableName);

            if (options != null && options.Count > 0)
            {
                sb.Append(" AND " + string.Join(" AND ", from pair in options
                                                         select string.Format("[{0}]=@{0}", pair.Key, pair.Key)));
            }

            if (idsList != null && idsList.Count > 0)
            {
                sb.AppendFormat(" AND {0} IN ({1})", AttributeNames.DynEntityId, string.Join(",", idsList));
            }

            if (!rowStart.HasValue || !rowsNumber.HasValue)
                sb.Append(" ) SELECT * FROM Assets ");
            else
            {
                sb.AppendFormat(" ) SELECT TOP {0} * FROM Assets ", rowsNumber);
                sb.AppendFormat("WHERE RowNumber > {0} ", rowStart);
            }

            sb.AppendFormat(" ORDER BY [{0}];", orderBy);

            var rows = new List<DynRow>(10);
            using (var reader = _unitOfWork.SqlProvider.ExecuteReader(sb.ToString(), (from item in options
                                                                                      select new SqlParameter()
                                                                                          {
                                                                                              ParameterName = item.Key,
                                                                                              Value = item.Value,
                                                                                          }).ToArray()))
            {
                while (reader.Read())
                {
                    var tableRow = new DynRow(assetConfig.DBTableName, (from attr in assetConfig.DynEntityAttribConfigs
                                                                        select _dynColumnAdapter.ConvertDynEntityAttribConfigToDynColumn(attr))
                                                                       .ToList());
                    foreach (var field in tableRow.Fields)
                    {
                        _assignValue(reader, field);
                    }
                    rows.Add(tableRow);
                }
                reader.Close();
            }
            return rows;
        }

        private static void _assignValue(IDataReader reader, DynColumn field)
        {
            try
            {
                if (reader[field.Name].GetType() != typeof(System.DBNull))
                    field.Value = Convert.ChangeType(reader[field.Name], field.DataType.FrameworkDataType, ApplicationSettings.PersistenceCultureInfo);
            }
            catch (FormatException)
            {
                field.Value = Convert.ToString(reader[field.Name], ApplicationSettings.PersistenceCultureInfo);
                //Debugger.Break();
            }
            catch (System.ArgumentOutOfRangeException noColumnException)
            {
                //Logger.Write(string.Format("Column {0} not found. Exception:\n{1}", field.Name, noColumnException.Message));
                Debugger.Break();
            }
            catch (Exception ex)
            {
                //Logger.Write(ex);
            }
        }

        /// <summary>
        /// Performs asset inserting
        /// </summary>
        ///<param name="table">Name of table to insert</param>
        ///<param name="asset">asset entity</param>
        public void InsertAsset(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException();

            var columns = (from attr in asset.Attributes
                           where attr.IsIdentity == false &&
                           !(string.IsNullOrEmpty(attr.Value) && attr.GetConfiguration().IsRequired == false && attr.GetConfiguration().IsAsset == false)
                           select _dynColumnAdapter.ConvertAttributeToDynColumn(attr)).ToList();

            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(asset.GetConfiguration().DBTableName);
            sb.Append(" ( ");
            sb.Append(columns.NamesToSQLString());
            sb.Append(" ) VALUES ( ");
            sb.Append(string.Join(", ", 
                columns.Select(c => _getVariableName(c))));
            sb.Append(" ) ");
            sb.Append("SELECT @@IDENTITY;");

            // inserting asset and
            // assigning new DynEntityUid to it

            var parameters = GetSqlParameters(columns);
            var result = _unitOfWork.SqlProvider.ExecuteScalar(sb.ToString(), parameters.ToArray());

            if (result == null)
                throw new Exception("Cannot retrieve IDENTITY");

            long lastInsertId;
            if (!long.TryParse(result.ToString(), out lastInsertId))
                throw new Exception("Cannot retrieve IDENTITY");

            _logger.DebugFormat("New entity with uid {1} inserted into [{0}]",
                asset.GetConfiguration().DBTableName, lastInsertId);

            asset.UID = lastInsertId;
        }

        /// <summary>
        /// Performs asset updating
        /// </summary>
        /// <param name="table">Name of table to update</param>
        /// <param name="asset">asset entity</param>
        public void UpdateAsset(Asset asset)
        {
            var sb = new StringBuilder();
            var columns = (from attr in asset.Attributes
                           where !attr.IsIdentity && !string.IsNullOrEmpty(attr.Value)
                           select _dynColumnAdapter.ConvertAttributeToDynColumn(attr)).ToList();
            // only on some locales
            sb.Append(@"UPDATE ");
            sb.Append(asset.GetConfiguration().DBTableName);
            sb.Append(" SET ");
            string valuesString = string.Join(", ", from column in columns
                                                    select string.Format("[{0}]={1}", column.Name, _getVariableName(column)));
            sb.Append(valuesString);
            sb.AppendFormat(" WHERE [{0}] = @{0};", AttributeNames.DynEntityUid);

            var parameters = GetSqlParameters(columns);
            parameters.Add(new SqlParameter("@" + AttributeNames.DynEntityUid, asset.Attributes.Single(g => g.IsIdentity).Value));

            var unitOfWork = new DataProxy.UnitOfWork();
            unitOfWork.SqlProvider.ExecuteNonQuery(sb.ToString(), parameters.ToArray());
        }

        private List<IDataParameter> GetSqlParameters(List<DynColumn> columns)
        {
            var parameters = new List<IDataParameter>(columns.Count);
            foreach (DynColumn column in columns)
            {
                string varibleName = _getVariableName(column);
                SqlDbType paramType;
                if (Enum.IsDefined(typeof(SqlDbType), DataTypeService.ConvertToDbDataType(column.DataType)))
                {
                    paramType = Routines.StringToEnum<SqlDbType>(DataTypeService.ConvertToDbDataType(column.DataType));
                }
                else
                {
                    paramType = SqlDbType.NVarChar;
                }

                // long strings trimming
                if (column.DataType.Code == Enumerators.DataType.String && column.Value != null && column.Value.ToString().Length > column.DataType.StringSize)
                {
                    column.Value = column.Value.ToString().Substring(0, (int)column.DataType.StringSize - 1);
                }

                var parameter = new SqlParameter(varibleName, paramType);
                parameter.Value = column.Value;
                parameters.Add(parameter);
            }
            return parameters;
        }

        private static string _getVariableName(DynColumn column)
        {
            return "@" + column.Name.Replace("-", "");
        }
    }
}
