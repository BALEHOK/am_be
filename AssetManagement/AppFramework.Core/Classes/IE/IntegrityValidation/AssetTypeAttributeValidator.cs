using AppFramework.ConstantsEnumerators;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    internal class AssetTypeAttributeValidator : IntegrityValidatorBase
    {
        public AssetTypeAttributeValidator(IUnitOfWork unitOfWork, ILayoutRepository layoutRepository)
            : base(unitOfWork, layoutRepository)
        {
            
        }

        /// <summary>
        /// Fixes the integrity of imported asset type attribute
        /// </summary>
        /// <param name="at"></param>
        public void Heal(AssetTypeAttribute attribute)
        {
            if (attribute.RelatedEntity.HasValue)
            {
                PredefinedAttribute pa = PredefinedAttribute.Get((PredefinedEntity)attribute.RelatedEntity);
                if (pa != null)
                {
                    attribute.RelatedAssetTypeID = pa.DynEntityConfigID;
                    attribute.RelatedAssetTypeAttributeID = pa.DynEntityAttribConfigID;
                }
            }
        }
    }
}
