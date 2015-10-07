namespace AppFramework.Core.Classes
{
    using System.Collections.Generic;
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes.Caching;
    using AppFramework.Entities;

    public class PredefinedAttribute
    {
        public string Name
        {
            get
            {
                return _base.Name;
            }
        }

        public long DynEntityConfigID
        {
            get
            {
                return _base.DynEntityConfigID;
            }
        }

        public long DynEntityAttribConfigID
        {
            get
            {
                return _base.DynEntityAttribConfigID;
            }
        }

        public PredefinedAttributes Base
        {
            get
            {
                return _base;
            }
        }

        private PredefinedAttributes _base;

        private PredefinedAttribute(PredefinedAttributes data)
        {
            _base = data;
        }

        /// <summary>
        /// Returns the PredefinedAttribute entity by its name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>DynEntityConfigUid in case predefinded attribute with such name exists, exception othercase</returns>
        public static PredefinedAttribute Get(PredefinedEntity entity)
        {
            var cache = CacheFactory.GetCache<PredefinedAttribute>("GetPredefinedAttributeByName");
            var data = cache.Get(entity.ToString());
            return data;
        }

        /// <summary>
        /// Checks if given asset type id is id of predefined entity
        /// </summary>
        /// <param name="assetTypeId"></param>
        /// <returns></returns>
        public static bool IsPredefined(long assetTypeId)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            return unitOfWork.PredefinedAttributesRepository.SingleOrDefault(pa => pa.DynEntityConfigID == assetTypeId) != null;
        }
        
        /// <summary>
        /// Returns the collection of predefined attributes 
        /// </summary>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<PredefinedAttribute> GetAll()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            foreach (PredefinedAttributes entity in unitOfWork.PredefinedAttributesRepository.Get())
            {
                yield return new PredefinedAttribute(entity);
            }
        }

        #region Cache methods
        [CacheKey("GetPredefinedAttributeByName")]
        public static string GetCacheKey(string predefinedEntityName)
        {
            return "PredefinedAttribute_" + predefinedEntityName;
        }

        [CacheValue("GetPredefinedAttributeByName")]
        public static PredefinedAttribute GetPredefinedAttributeByNameDb(string predefinedEntityName)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var data = unitOfWork.PredefinedAttributesRepository.SingleOrDefault(pa => pa.Name == predefinedEntityName);
            return data == null ? null : new PredefinedAttribute(data);
        }
        #endregion
    }
}
