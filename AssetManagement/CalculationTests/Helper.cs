using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;

namespace CalculationTests
{
    public class Helper
    {
        private readonly AppTest _app;

        public Helper(AppTest appTest)
        {
            _app = appTest;
        }

        public List<Asset> GetActiveAssetsByName(string tableName, string assetName)
        {
            var type = GetTypeByName(tableName);
            var assets = _app.AssetsService.GetAssetsByParameters(type, new Dictionary<string, string>
            {
                {AttributeNames.Name, assetName},
                {AttributeNames.ActiveVersion, bool.TrueString},
            }).ToList();
            return assets;
        }

        public AssetType GetTypeByName(string tableName)
        {
            var config = _app.UnitOfWork.DynEntityConfigRepository.Get(c => c.DBTableName == tableName && c.ActiveVersion,
                include: c => c.DynEntityAttribConfigs).Single();
            var type = new AssetType(config, _app.UnitOfWork);
            return type;
        }

        public Asset CreateAsset(string tableName, string assetName)
        {
            var asset = _app.AssetsService.CreateAsset(GetTypeByName(tableName));
            asset["Name"].Value = assetName;
            return asset;
        }
    }
}
