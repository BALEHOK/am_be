using System;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.Classes.Batch.AssetActions
{
    class LocationMove : BatchAction
    {
        private readonly IAssetsService _assetsService;
        public LocationMove(AppFramework.Entities.BatchAction batchAction, IAssetsService assetsService)
            : base(batchAction)
        {
            if (assetsService == null)
                throw new ArgumentNullException("IAssetsService");
            _assetsService = assetsService;
        }

        public override void Run()
        {
            List<long> assetIDS = new List<long>();
            List<long> locastionIds = new List<long>();

            string[] strLocIds = this.Parameters["NewLocations"].ToString().Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string locId in strLocIds)
                locastionIds.Add(long.Parse(locId));

            string[] strIDS = this.Parameters["AssetTypes"].ToString().Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sid in strIDS)
                assetIDS.Add(long.Parse(sid));

            foreach (long LocationId in locastionIds)
                foreach (long assetTypeId in assetIDS)
                {
                    var assetsToMove = _assetsService.GetAssetsByAssetTypeIdAndLocation(assetTypeId, LocationId).ToList();
                    for (int i = 0; i < assetsToMove.Count; i++)
                    {
                        assetsToMove[i].MoveToNextLocation();
                        _assetsService.InsertAsset(assetsToMove[i]);
                    }
                }
        }
    }
}
