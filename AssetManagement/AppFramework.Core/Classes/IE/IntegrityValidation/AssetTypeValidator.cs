using System;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    using System.Linq;

    internal class AssetTypeValidator : IntegrityValidatorBase
    {

        public AssetTypeValidator(IUnitOfWork unitOfWork, ILayoutRepository layoutRepository)
            : base(unitOfWork, layoutRepository)
        {
            
        }

        /// <summary>
        /// Fixes the integrity of imported asset type
        /// </summary>
        /// <param name="at"></param>
        public void Heal(AssetType at)
        {
            // attributes validation
            var attributeValidator = new AssetTypeAttributeValidator(_unitOfWork, _layoutRepository);
            foreach (var attr in (at).Attributes)
                attributeValidator.Heal(attr);

            // there is panels but no one attribute was assigned
            if (at.Panels.Count > 0 && at.Panels.All(p => p.AssignedAttributes.Count == 0))
            {
                var pnl = at.Panels.First();
                at.Attributes.ToList()
                             .ForEach(pnl.AssignAssetTypeAttribute);
            }
        }
    }
}
