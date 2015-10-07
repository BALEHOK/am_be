using System.Collections.Generic;
using System.Data.SqlClient;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;

namespace AppFramework.Core.DAL
{
    public interface ITableProvider
    {
        DynRow GetRowById(Entities.DynEntityConfig assetConfig, long assetId);
        DynRow GetRowByUid(Entities.DynEntityConfig assetConfig, long assetUid);
        DynRow GetRowByIdAndRevision(Entities.DynEntityConfig assetConfig, long assetId, int revision);

        void InsertAsset(Asset asset);

        List<DynRow> GetRows(long typeId, Dictionary<string, string> options = null, string orderBy = null,
            bool desc = false, Dictionary<long, AssetType> typeRevisions = null);

        List<DynRow> GetRows(Entities.DynEntityConfig assetConfig,
            List<SqlParameter> options = null,
            long? rowStart = null,
            long? rowsNumber = null,
            string orderBy = AttributeNames.Name,
            List<long> idsList = null);

        List<DynRow> GetRows(Entities.DynEntityConfig assetConfig,
            Dictionary<string, string> options = null,
            long? rowStart = null,
            long? rowsNumber = null,
            string orderBy = AttributeNames.Name,
            List<long> idsList = null);

        List<DynRow> GetAllRowsByAssetConfiguration(Entities.DynEntityConfig assetConfig, bool IsActiveVersion,
            long? rowStart = null, long? rowsNumber = null);

        DynRow GetFirstActiveRow(Entities.DynEntityConfig assetConfig);
    }
}
