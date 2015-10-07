using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using Common.Logging;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace AppFramework.Core.Classes
{
    public interface ILinkedEntityFinder
    {
        /// <summary>
        /// Returns asset id by node value. Could be as Id as Name of asset.
        /// </summary>
        /// <param name="assetTypeId">ID of AT which is relates to current attribute</param>
        /// <param name="assetTypeAttributeId">ID of ATA which is the displayed field</param>
        /// <param name="value"></param>
        /// <returns></returns>
        long FindRelatedAssetId(AssetType assetType, long assetTypeAttributeId, string value);

        string GetRelatedAssetDisplayName(long assetTypeId, long assetTypeAttributeId, long assetId, bool activeVersion);

        /// <summary>
        /// Finds an asset in DynEntityIndex table by its name
        /// </summary>
        /// <param name="atId"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        long FindAssetInIndex(long atId, string nodeValue);
    }

    public class LinkedEntityFinder : ILinkedEntityFinder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILog _logger;

        //TODO: logger shouldn't be null
        public LinkedEntityFinder(IUnitOfWork unitOfWork, ILog logger = null)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
      
        public long FindRelatedAssetId(AssetType assetType, long assetTypeAttributeId, string value)
        {
            if (assetType == null)
                throw new ArgumentNullException("AssetType");
            var attribute = assetType.Attributes.SingleOrDefault(a => a.ID == assetTypeAttributeId);
            if (attribute == null)
                throw new ArgumentException(string.Format("Cannot find attribute of {0} with id {1}",
                    assetType.NameInvariant, assetTypeAttributeId));

            var sql = string.Format("SELECT {0} FROM [{1}] WHERE [{2}] = @value",
                AttributeNames.DynEntityId, assetType.DBTableName, attribute.DBTableFieldName);
            if (_logger != null)
                _logger.DebugFormat("Searching for related asset by value '{0}' with statement: {1}", value, sql);
            object result = null;

            try
            {
                result = _unitOfWork.SqlProvider.ExecuteScalar(sql, new SqlParameter[]
                {
                    new SqlParameter("@value", value)
                });
            }
            catch (SqlException ex)
            {
                throw new FieldLinkingException(
                    string.Format("Cannot find [{2}] by [{0}] = '{1}'",
                    attribute.DBTableFieldName, value, assetType.DBTableName));
            }

            if (result == null)
                throw new FieldLinkingException(
                    string.Format("Cannot find [{2}] by [{0}] = '{1}'",
                    attribute.DBTableFieldName, value, assetType.DBTableName));

            return (long) result;
        }
      
        public long FindAssetInIndex(long atId, string nodeValue)
        {
            var result = _unitOfWork.DynEntityIndexRepository
                .SingleOrDefault(idx =>
                    idx.DynEntityConfigId == atId &&
                    idx.Name == nodeValue);
            return result != null 
                ? result.DynEntityId 
                : default(long);
        }

        public string GetRelatedAssetDisplayName(long assetTypeId, long assetTypeAttributeId, long assetId,
            bool activeVersion)
        {
            if (assetTypeId == 0 || assetTypeAttributeId == 0 || assetId == 0)
                throw new ArgumentException();

            var outputParameter = new SqlParameter("@result", string.Empty)
            {
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Size = 4000
            };

            _unitOfWork.SqlProvider.ExecuteNonQuery(StoredProcedures.GetValueByAT_ATA_A,
                new SqlParameter[]
                {
                    new SqlParameter("@AssetTypeID", assetTypeId) {SqlDbType = System.Data.SqlDbType.BigInt},
                    new SqlParameter("@AssetTypeAttributeID", assetTypeAttributeId)
                    {
                        SqlDbType = System.Data.SqlDbType.BigInt
                    },
                    new SqlParameter("@AssetID", assetId) {SqlDbType = System.Data.SqlDbType.BigInt},
                    new SqlParameter("@isUniqueId", !activeVersion) {SqlDbType = System.Data.SqlDbType.Bit},
                    outputParameter
                }, System.Data.CommandType.StoredProcedure);

            return ((outputParameter.Value as string) != null)
                ? (string) outputParameter.Value
                : string.Empty;
        }
    }
}