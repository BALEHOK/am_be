using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    internal class AssetTypeIntegrityValidator : IntegrityValidatorBase, IIntegrityValidator<AssetType>
    {

        public AssetTypeIntegrityValidator() { }

        #region IIntegrityValidator<AssetType> Members

        public bool Validate(AssetType obj)
        {
            bool res = true;
           
            // validate type
            int cTypeId;
            res &= ValidateType(obj.TypeId, out cTypeId);
            obj.TypeId = cTypeId;
            
            // validate context
            if (obj.ContextId != null)
            {
                long cContextId;
                res &= ValidateContext((long)obj.ContextId, out cContextId);
                obj.ContextId = cContextId;
            }

            // validate layout
            int cLayoutId;
            res &= ValidateLayout(obj.LayoutId, out cLayoutId);
            obj.LayoutId = cLayoutId;
            
            if (AllowErrorsCorrection)
            {
                return true;
            }
            else 
            {
                return res;
            }
            
            return res;
        }

        public bool IsNew
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
