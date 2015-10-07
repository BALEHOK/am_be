using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.DynamicEntity.Entities;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    /// <summary>
    /// Validates the integrity ob an object comparing with DB data
    /// </summary>
    public class IntegrityValidationService
    {

        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        public IntegrityValidationService() { }
        
        public bool Validate(object obj)
        {
            bool res = true;
            if (obj.GetType() == typeof(AssetType))
            {
                AssetType at = obj as AssetType;
                AssetTypeIntegrityValidator validator = new AssetTypeIntegrityValidator();
                res &= validator.Validate(at);

                // there is panels but no one attribute was assigned
                if (at.Panels.Count > 0 && at.Panels.All(p => p.AssignedAttributes.Count == 0))
                {
                    Panel pnl = at.Panels.First();
                    at.Attributes.ForEach(a => pnl.AssignAttribute(a));
                }

                // attributes validation
                foreach (AssetTypeAttribute attr in (at).Attributes)
                {
                    // if it's a new object, assign fake UID's to attributes
                    if (at.Base.IsNew)
                    {
                        attr.UID = rnd.Next();
                    }

                    res &= this.Validate(attr);
                }
            }
            else if (obj.GetType() == typeof(AssetTypeAttribute))
            {
                AssetTypeAttributeIntegrityValidator validator = new AssetTypeAttributeIntegrityValidator();
                //res &= validator.Validatate(obj as AssetTypeAttribute);
                //SetPredefinedDependencies(obj as AssetTypeAttribute);
            }
            else if (obj.GetType() == typeof(Asset))
            {
                AssetValidator validator = new AssetValidator();
                res &= validator.Validate(obj as Asset);
            }
            else if (obj.GetType() == typeof(EntityContext))
            {

            }
            else if (obj.GetType() == typeof(EntityContextType))
            {

            }
            else if (obj.GetType() == typeof(Layout))
            {

            }
            else
            {
                throw new ArgumentException("Unknown object type");
            }
            return res;
            //return true;
        }

        /// <summary>
        /// Sets the related ID's for attributes of predefined types
        /// </summary>
        /// <param name="attribute"></param>
        private void SetPredefinedDependencies(AssetTypeAttribute attribute)
        {
            if (attribute.RelatedEntity.HasValue)
            {
                PredefinedAttributes pa = PredefinedAttribute.Get(attribute.RelatedEntity.ToString());
                attribute.RelatedAssetTypeID = pa.DynEntityConfigId;
                attribute.RelatedAssetTypeAttributeID = pa.DynEntityAttribConfigId;
            }                     
        }
    }
}
