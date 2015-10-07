using AppFramework.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.Batch
{
    public class SyncAssetsParameters : BatchActionParameters
    {
        public string AssetIdentifier
        {
            get { return this[ImportExportParameter.AssetIdentifier.ToString()].ToString(); }
        }

        public long UserID
        {
            get { return long.Parse(this[ImportExportParameter.UserID.ToString()].ToString()); }
        }

        public long AssetTypeId
        {
            get { return long.Parse(this[ImportExportParameter.AssetTypeId.ToString()].ToString()); }
        }

        public SyncAssetsParameters(Guid guid, long userId, long assetTypeId, string assetIdentifier, string sheets, string filePath)
        {
            this.Add(ImportExportParameter.Guid.ToString(), guid.ToString());
            this.Add(ImportExportParameter.UserID.ToString(), userId.ToString());
            this.Add(ImportExportParameter.AssetTypeId.ToString(), assetTypeId.ToString());
            this.Add(ImportExportParameter.AssetIdentifier.ToString(), assetIdentifier);
            this.Add(ImportExportParameter.Sheets.ToString(), sheets);
            this.Add(ImportExportParameter.FilePath.ToString(), filePath);
        }
    }
}
