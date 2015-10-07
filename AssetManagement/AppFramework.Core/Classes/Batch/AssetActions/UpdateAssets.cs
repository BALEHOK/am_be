using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace AppFramework.Core.Classes.Batch.AssetActions
{
    public class UpdateAssets : BatchAction
    {
        private readonly IAssetsService _assetsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public UpdateAssets(
            Entities.BatchAction batchAction, 
            IUnitOfWork unitOfWork, 
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
            : base(batchAction)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("IUnitOfWork");
            _unitOfWork = unitOfWork;

            if (assetsService == null)
                throw new ArgumentNullException("IAssetsService");
            _assetsService = assetsService;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
        }

        public override void Run()
        {
            if (Parameters["attributeElements"] == null)
                throw new ArgumentNullException("attributeElements");
            if (Parameters["searchid"] == null)
                throw new ArgumentNullException("searchid");
            if (Parameters["typeuid"] == null)
                throw new ArgumentNullException("typeuid");

            var searchId = long.Parse(Parameters["searchid"].ToString());
            var assetTypeId = long.Parse(Parameters["typeuid"].ToString());
            var userId = long.Parse(Parameters[ImportExportParameter.UserID.ToString()].ToString());

            var assetType = _assetTypeRepository.GetByUid(assetTypeId);
            var oneType = new OneType(assetType, _unitOfWork, _assetsService, _assetTypeRepository, true);

            var deserializedAttributes = JsonConvert.DeserializeObject<List<AttributeElement>>(
                Parameters["attributeElements"]);
            var query = string.Format("select DynEntityUid from {0} where SearchId = {1}", 
                Constants.TypeContextSearchTempTable, searchId);

            var unitOfWork = new UnitOfWork();
            using (var dbReader = unitOfWork.SqlProvider.ExecuteReader(query))
            {
                while (dbReader.Read())
                {
                    var assetId = long.Parse(dbReader[0].ToString());
                    //get asset by uid
                    var asset = _assetsService.GetAssetByUid(assetId, assetType);
                    //update attributes
                    foreach (var deserializedObject in deserializedAttributes)
                    {
                        var foundAttribute = asset.Attributes
                            .SingleOrDefault(attr => attr.GetConfiguration().DBTableFieldName
                                                     == oneType.Attributes[deserializedObject.AttributeId]
                                                         .Configuration.DBTableFieldName);
                        if (foundAttribute == null)
                            continue;
                        foundAttribute.InitData();
                        if (deserializedObject.DateType == Enumerators.DataType.DynList)
                        {
                            var config = foundAttribute.GetConfiguration();
                            var item = config.DynamicList.Items
                                .FirstOrDefault(t => t.UID == long.Parse(deserializedObject.Value));

                            var v = new DynamicListValue()
                            {
                                ParentListId = 0,
                                DynamicListUid = foundAttribute.Configuration.DynamicListUid.Value,
                                DynamicListItemUid = long.Parse(deserializedObject.Value),
                                Value = deserializedObject.Value
                            };

                            foundAttribute.DynamicListValues.Clear();
                            foundAttribute.DynamicListValues.Add(v);
                        }
                        else if (!string.IsNullOrEmpty(deserializedObject.Value))
                        {
                            foundAttribute.ValueAsId = long.Parse(deserializedObject.Value);
                        }
                        foundAttribute.Value = deserializedObject.Text;
                    }

                    //save asset
                    asset[AttributeNames.UpdateUserId].Value = userId.ToString();
                    asset[AttributeNames.UpdateUserId].ValueAsId = userId;
                    asset[AttributeNames.ActiveVersion].Value = "1";
                    asset[AttributeNames.UpdateDate].Value =
                        DateTime.Now.ToString(ApplicationSettings.DisplayCultureInfo.DateTimeFormat);
                    _assetsService.InsertAsset(asset);
                }
            }
        }
    }
}
