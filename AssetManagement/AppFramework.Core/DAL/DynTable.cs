﻿using AppFramework.Core.Classes;
using AppFramework.Core.DAL.Adapters;
using AppFramework.DataProxy;
using System;
using System.Linq;
using System.Text;

namespace AppFramework.Core.DAL
{
    public class DynTable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDynColumnAdapter _dynColumnAdapter;

        /// <summary>
        /// Creates a new instance of DynTable with full configured set of columns
        /// </summary>
        public DynTable(
            IDynColumnAdapter dynColumnAdapter,
            IUnitOfWork unitOfWork)
        {
            if (dynColumnAdapter == null)
                throw new ArgumentNullException("IDynColumnAdapter");
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");

            _unitOfWork = unitOfWork;
            _dynColumnAdapter = dynColumnAdapter;
        }

        /// <summary>
        /// Creates the database table
        /// </summary>
        public void Create(AssetType at)
        {
            if (at == null)
                throw new ArgumentNullException("AssetType");

            string tableName = at.DBTableName;
            if (DBHelper.IsTableExists(_unitOfWork, tableName))
                throw new Exception(
                    string.Format("Table {0} already exists. Cannot create table for asset type {1}.",
                    tableName, at.Name));

            var row = new DynRow(
                at.DBTableName,
                at.Attributes.Select(attr => _dynColumnAdapter.ConvertDynEntityAttribConfigToDynColumn(attr.Base)));
        
            var sb = new StringBuilder();
            sb.Append("IF NOT EXISTS ");
            sb.Append(" ( SELECT * FROM INFORMATION_SCHEMA.TABLES ");
            sb.AppendFormat(" WHERE TABLE_NAME = '{0}' )", tableName);
            sb.AppendFormat(" CREATE TABLE {0} ( ", tableName);
            sb.Append(string.Join(", ", from column in row.Fields
                                        select GetSql(column)));
            sb.Append(" ); ");

            //Creates a new database table if it's not exists
            _unitOfWork.SqlProvider.ExecuteNonQuery(sb.ToString(), null);

            // create triggers and indexes on this table
            _unitOfWork.RebuildTriggers(at.UID);
        }

        /// <summary>
        /// Gets SQL description of column
        /// </summary>
        private string GetSql(DynColumn column)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("[{0}]", column.Name));
            sb.Append(" ");
            sb.Append(DataTypeService.ConvertToDbDataType(column.DataType));
            sb.Append(column.IsNull ? " NULL " : " NOT NULL ");

            if (column.IsIdentity)
            {
                sb.Append(" PRIMARY KEY IDENTITY(1,1) ");
            }
            else
            {
                if (!column.IsNull)
                {
                    sb.Append(" DEFAULT ");
                    sb.Append(column.GetDefaultValue());
                }
            }
            return sb.ToString();
        }
    }
}
