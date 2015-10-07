using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    internal class AssetTypeAttributeIntegrityValidator : IntegrityValidatorBase, IIntegrityValidator<AssetTypeAttribute>
    {

        #region IIntegrityValidator Members

        public bool Validate(AssetTypeAttribute obj)
        {            
            bool res = true;

            // check item ID and UID, set the IsNew flag

            // check the parent AssetType existing by UID

            // check the field existing in table by DBTableFieldName

            // check datatype existing by Uid

            // check Context existing by Id
            
            // check DynList existing by Uid

            // check the RelatedAssetTypeID existing
            // check the RelatedAssetTypeAttributeID existing
      
            throw new NotImplementedException();
        }

        public bool IsNew
        {
            get { throw new NotImplementedException(); }
            private set { throw new NotImplementedException(); }
        }


        #endregion

    }
}
