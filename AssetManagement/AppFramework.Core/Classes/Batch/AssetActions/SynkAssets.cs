using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.Classes.IE.Adapters;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.Helpers;
using AppFramework.DataProxy;
using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace AppFramework.Core.Classes.Batch.AssetActions
{
    public class SynkAssets : BatchAction
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IExcelToXmlConverter _excelToXmlAdapter;
        private readonly IAssetsService _assetsService;
        private readonly IBarcodeProvider _barcodeProvider;

        public SynkAssets(Entities.BatchAction batchAction, 
            IUnitOfWork unitOfWork, 
            IAssetsService assetsService, 
            IAssetTypeRepository assetTypeRepository,
            IBarcodeProvider barcodeProvider)
            : base(batchAction)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            if (assetsService == null)
                throw new ArgumentNullException("IAssetsService");
            if (assetTypeRepository == null)
                throw new ArgumentNullException("IAssetTypeRepository");
            if (barcodeProvider == null)
                throw new ArgumentNullException("barcodeProvider");
            _barcodeProvider = barcodeProvider;
            _unitOfWork = unitOfWork;
            _assetsService = assetsService;
            _assetTypeRepository = assetTypeRepository;
            _excelToXmlAdapter = new ExcelToXmlConverter();
        }

        public class AssetComparer : IEqualityComparer<Asset>
        {
            private string assetIdentifier;

            public AssetComparer(string assetIdentifier)
            {
                this.assetIdentifier = assetIdentifier;
            }

            public bool Equals(Asset x, Asset y)
            {
                AssetAttribute attributeX = x.Attributes.SingleOrDefault(a => a.GetConfiguration().DBTableFieldName.ToLower() == assetIdentifier.ToLower());
                AssetAttribute attributeY = y.Attributes.SingleOrDefault(a => a.GetConfiguration().DBTableFieldName.ToLower() == assetIdentifier.ToLower());

                if (attributeX.ValueAsId.HasValue)
                {
                    return (attributeX.ValueAsId == attributeY.ValueAsId);
                }

                return attributeX.Value == attributeY.Value;
            }

            public int GetHashCode(Asset obj)
            {
                AssetAttribute attribute = obj.Attributes.SingleOrDefault(a => a.GetConfiguration().DBTableFieldName.ToLower() == assetIdentifier.ToLower());
                if (attribute.ValueAsId.HasValue)
                {
                    return (int)attribute.ValueAsId.Value;
                }

                return attribute.Value.GetHashCode();
            }
        }

        public override void Run()
        {
            IE.StatusInfo status = new IE.StatusInfo();

            // retrieve the task entry guid
            Guid guid = new Guid(this.Parameters[ImportExportParameter.Guid.ToString()].ToString());
            long userId = long.Parse(this.Parameters[ImportExportParameter.UserID.ToString()].ToString());

            string assetIdentifier = this.Parameters[ImportExportParameter.AssetIdentifier.ToString()].ToString();
            if (String.IsNullOrEmpty(assetIdentifier))
            {
                throw new ArgumentException("Asset identifier for synk was not provided");
            }

            // get import/export task entry
            var dbEntity = _unitOfWork.ImportExportRepository.Single(ie => ie.GUID == guid);

            // get task parameters
            IE.ImportExportParameters importParams
                = IE.ImportExportParameters.GetFromXml(dbEntity.Parameters);

            // retrieve assetTypeId 
            long assetTypeId;
            if (!long.TryParse(importParams[ImportExportParameter.AssetTypeId], out assetTypeId))
            {
                throw new ArgumentException("AssetTypeId was not provided");
            }

            var assetType = _assetTypeRepository.GetById(assetTypeId);
            var sourceFilePath = this.Parameters[ImportExportParameter.FilePath.ToString()];
            var bindings = IE.BindingInfo.GetFromXml(dbEntity.Bindings);
            var sheets = new List<string>();
            CustomSerializers.Deserialize(sheets, this.Parameters[ImportExportParameter.Sheets.ToString()].ToString());
            var xmlExport = _excelToXmlAdapter.ConvertToXml(sourceFilePath, bindings, assetType, sheets);

            // get assets from XML file
            var adapter = new XMLToAssetsAdapter(
                _assetsService,
                _assetTypeRepository,
                new LinkedEntityFinder(_unitOfWork),
                _barcodeProvider);
            var result = adapter.GetEntities(xmlExport, assetType);

            foreach (var asset in result)
            {
                var attribute = asset.Attributes
                    .SingleOrDefault(a => a.GetConfiguration().DBTableFieldName.ToLower() 
                        == assetIdentifier.ToLower());

                if (attribute == null)
                    throw  new NullReferenceException(
                        string.Format("Cannot find field {0}", assetIdentifier));

                if (!attribute.ValueAsId.HasValue && string.IsNullOrEmpty(attribute.Value))
                    continue;

                Asset existingAsset = null;

                switch (attribute.Configuration.DataTypeEnum)
                {
                    case Enumerators.DataType.String:
                    case Enumerators.DataType.Text:
                        if (!String.IsNullOrEmpty(attribute.Value))
                        {
                            existingAsset = _assetsService.GetAssetByParameters
                            (
                                assetType,
                                new List<SqlParameter>() 
                                    { 
                                        { new SqlParameter(){ ParameterName = assetIdentifier, Value = attribute.Value.ToString(), DbType = System.Data.DbType.String }},
                                        { new SqlParameter(){ ParameterName = AttributeNames.ActiveVersion, Value = bool.TrueString, DbType = System.Data.DbType.Boolean }}
                                    }
                            );
                        }
                        break;
                    default:
                        if (!String.IsNullOrEmpty(attribute.Value))
                        {
                            if (attribute.ValueAsId.HasValue)
                            {
                                existingAsset = _assetsService.GetAssetByParameters
                                    (
                                        assetType,
                                        new Dictionary<string, string>() { 
                                            { assetIdentifier, attribute.ValueAsId.Value.ToString() },
                                            { AttributeNames.ActiveVersion, bool.TrueString }
                                        }
                                );
                            }
                            else
                            {
                                existingAsset = _assetsService.GetAssetByParameters
                                    (
                                        assetType,
                                        new Dictionary<string, string>() { 
                                            { assetIdentifier, attribute.Value.ToString() },
                                            {AttributeNames.ActiveVersion, bool.TrueString  }
                                        }
                                );
                            }
                        }
                        break;
                }

                //var uof = new UnitOfWork();
                asset[AttributeNames.UpdateUserId].Value = userId.ToString();
                asset[AttributeNames.UpdateUserId].ValueAsId = userId;

                asset[AttributeNames.ActiveVersion].Value = "1";
                asset[AttributeNames.UpdateDate].Value = DateTime.Now.ToString(ApplicationSettings.DisplayCultureInfo.DateTimeFormat);


                //fix datetime attributes
                foreach (AssetAttribute attr in asset.Attributes)
                {
                    if (attr.GetConfiguration().DataTypeEnum == Enumerators.DataType.DateTime && attr.GetConfiguration().IsRequired)
                    {
                        if (String.IsNullOrEmpty(attr.Value))
                        {
                            attr.Value = DateTime.Now.ToString(ApplicationSettings.DisplayCultureInfo.DateTimeFormat);
                        }
                    }
                }

                if (assetType.DBTableName == Enumerators.DBTableNames.ADynEntityUser.ToString())
                {
                    if (existingAsset != null && existingAsset[AttributeNames.Role].Value != ((int)PredefinedRoles.OnlyPerson).ToString())
                    {
                        //this is not ONLY PERSON skip!
                        continue;
                    }

                    asset[AttributeNames.Role].Value = ((int)PredefinedRoles.OnlyPerson).ToString();

                    asset[AttributeNames.PermissionOnUsers].Value = "0"; //TODO: Refactor 0 - no permissions for person only
                }

                if (existingAsset == null)
                {
                    asset[AttributeNames.DynEntityId].Value = "0";
                    asset[AttributeNames.DynEntityUid].Value = "0";
                    asset[AttributeNames.DynEntityId].ValueAsId = 0;
                    asset[AttributeNames.DynEntityUid].ValueAsId = 0;
                }
                else
                {
                    asset[AttributeNames.DynEntityId].Value = existingAsset[AttributeNames.DynEntityId].Value;
                    asset[AttributeNames.DynEntityUid].Value = existingAsset[AttributeNames.DynEntityUid].Value;
                    asset[AttributeNames.DynEntityId].ValueAsId = existingAsset[AttributeNames.DynEntityId].ValueAsId;
                    asset[AttributeNames.DynEntityUid].ValueAsId = existingAsset[AttributeNames.DynEntityUid].ValueAsId;
                }

                using (var transactionScope = new TransactionScope())
                {
                    _assetsService.InsertAsset(asset);

                    if (existingAsset != null && AssetExtentions.IsAssetEqual(existingAsset, asset, new String[18]{
                        AttributeNames.ActiveVersion,
                        AttributeNames.DynEntityConfigId,
                        AttributeNames.DynEntityConfigUid,
                        AttributeNames.DynEntityId,
                        AttributeNames.DynEntityUid,
                        AttributeNames.Revision,
                        AttributeNames.Barcode,
                        AttributeNames.UpdateDate,
                        AttributeNames.UpdateDateEx,
                        AttributeNames.UpdateUserId,
                        AttributeNames.Revision,
                        AttributeNames.UserId,
                        AttributeNames.LastLoginDate,
                        AttributeNames.LastActivityDate,
                        AttributeNames.LastLockoutDate,
                        AttributeNames.Role,
                        AttributeNames.OwnerId,
                        AttributeNames.Owner
                        }))
                    {
                        //nothing to do
                        //asset is equal
                        //uof.Dispose();
                        continue;
                    }

                    //uof.Commit();
                    transactionScope.Complete();
                }
            }

            //find assets to delete
            var existingAssets = _assetsService.GetAssetsByParameters(
                            assetType,
                            new Dictionary<string, string>() { 
                                    {AttributeNames.DynEntityConfigUid, assetType.UID.ToString()},
                                    {AttributeNames.ActiveVersion, bool.TrueString  }
                                }
            );

            var assetsToDelete = existingAssets.Except(result, new AssetComparer(assetIdentifier));

            foreach (var asset in assetsToDelete)
            {
                var attr = (asset.Attributes.SingleOrDefault(x => x.Configuration.DBTableFieldName.ToLower() == assetIdentifier.ToLower()));

                //TODO Refactor;
                if ((!attr.ValueAsId.HasValue || attr.ValueAsId.Value == 0) && String.IsNullOrEmpty(attr.Value))
                {
                    continue;
                }

                asset[AttributeNames.ActiveVersion].Value = false.ToString();
                asset.IsDeleted = true;
                _assetsService.InsertAsset(asset);
            }

            dbEntity.Status = status.IsSuccess ? (int)ImportExportStatus.Completed : (int)ImportExportStatus.Error;

            if (status.Errors.Count > 0)
            {
                dbEntity.Message += "\nErrors:\n" + string.Join("\n", status.Errors.ToArray());
            }
            if (status.Warnings.Count > 0)
            {
                dbEntity.Message += "\nWarnings:\n" + string.Join("\n", status.Warnings.ToArray());
            }

            // update synk task entry   
            _unitOfWork.ImportExportRepository.Update(dbEntity);
            _unitOfWork.Commit();
        }
    }

    public static class AssetExtentions
    {
        public static bool IsAssetEqual(Asset asset1, Asset asset2, params string[] ignore)
        {
            bool isEqual = false;

            if (asset1 != null && asset2 != null)
            {
                isEqual = true;

                for (int i = 0; i < asset1.AttributesPublic.Count && isEqual; i++)
                {
                    if (ignore.Contains(asset1.AttributesPublic[i].Configuration.Name))
                    {
                        continue;
                    }

                    switch (asset1.AttributesPublic[i].Configuration.DataTypeEnum)
                    {
                        case Enumerators.DataType.DateTime:
                            isEqual = DateTime.Parse(asset1.AttributesPublic[i].Value) == DateTime.Parse(asset2.AttributesPublic[i].Value);
                            break;
                        case Enumerators.DataType.DynList:
                        case Enumerators.DataType.DynLists:
                        case Enumerators.DataType.Asset:
                            var val1 = asset1.AttributesPublic[i].ValueAsId;
                            var val2 = asset2.AttributesPublic[i].ValueAsId;

                            if (val1.HasValue && val2.HasValue)
                            {
                                isEqual = asset1.AttributesPublic[i].ValueAsId == asset2.AttributesPublic[i].ValueAsId;
                            }

                            if (asset1.AttributesPublic[i].DynamicListValues != null)
                            {
                                if (asset2.AttributesPublic[i].DynamicListValues == null)
                                {
                                    isEqual = false;
                                    break;
                                }

                                if (asset1.AttributesPublic[i].DynamicListValues.Count != asset2.AttributesPublic[i].DynamicListValues.Count)
                                {
                                    isEqual = false;
                                    break;
                                }

                                for (int j = 0; j < asset1.AttributesPublic[i].DynamicListValues.Count; j++)
                                {
                                    isEqual = asset1.AttributesPublic[i].DynamicListValues[j].DynamicListItemUid == asset2.AttributesPublic[i].DynamicListValues[j].DynamicListItemUid &&
                                        asset1.AttributesPublic[i].DynamicListValues[j].DynamicListUid == asset2.AttributesPublic[i].DynamicListValues[j].DynamicListUid &&
                                        asset1.AttributesPublic[i].DynamicListValues[j].Value == asset2.AttributesPublic[i].DynamicListValues[j].Value;
                                }
                            }
                            break;
                        default:
                            isEqual = asset1.AttributesPublic[i].Value == asset2.AttributesPublic[i].Value;
                            break;
                    }
                }
            }

            return isEqual;
        }
    }
}
