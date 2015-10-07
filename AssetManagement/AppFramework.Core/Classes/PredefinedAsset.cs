/*--------------------------------------------------------
* PredefinedAsset.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/14/2009 12:39:33 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace AppFramework.Core.Classes
{
    using AppFramework.Core.Classes.Caching;

    public class PredefinedAsset
    {
        /// <summary>
        /// Checks if given asset is predefined or not.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool Contains(Asset asset)
        {
            var cache = CacheFactory.GetCache<PredefinedAsset>("UID");
            var entity = cache.Get(asset.GetConfiguration().ID, asset.ID);
            return entity != null;
        }

        [CacheKey("UID")]
        public static string GetCacheKey(long dynEntityConfigId, long dynEntityId)
        {
            return string.Format("PredefinedAsset_DynEntityConfigId_{0}_DynEntityId_{1}", dynEntityConfigId, dynEntityId);
        }

        [CacheValue("UID")]
        public static Entities.PredefinedAsset GetByDynEntityConfigIdDynEntityIdDb(long dynEntityConfigId, long dynEntityId)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            return unitOfWork.PredefinedAssetRepository.SingleOrDefault(p => p.DynEntityConfigId == dynEntityConfigId && p.DynEntityId == dynEntityId);
        }

        /// <summary>
        /// Sets given asset as predefined.
        /// </summary>
        /// <param name="asset"></param>
        public static void Add(Asset asset)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            unitOfWork.PredefinedAssetRepository.Insert(new Entities.PredefinedAsset()
            {
                DynEntityConfigId = asset.GetConfiguration().ID,
                DynEntityId = asset.ID,
                PredefinedAssetName = asset.Name
            });
            unitOfWork.Commit();
        }
    }
}
