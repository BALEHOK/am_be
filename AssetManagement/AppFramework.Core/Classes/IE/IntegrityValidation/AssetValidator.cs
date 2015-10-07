using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    public class AssetValidator : IntegrityValidatorBase
    {
        public AssetValidator() { }

        #region IIntegrityValidator<Asset> Members

        public bool Validate(Asset obj)
        {
            bool res = true;
            
            foreach (AssetAttribute attribute in obj.Attributes)
            {
                if (attribute.GetConfiguration().IsRequired)
                {
                    switch (attribute.DataType.Code)
                    {
                        case Enumerators.DataType.DynList:
                        case Enumerators.DataType.DynLists:
                            res &= attribute.DynamicListValues.Count() > 0;
                            break;

                        case Enumerators.DataType.Asset:
                            res &= attribute.ValueAsID != 0;
                            break; 

                        case Enumerators.DataType.Assets:
                            res &= attribute.MultipleAssets.Count > 0;
                            break;

                        default:
                            res &= attribute.Value != string.Empty;
                            break;
                    }

                }
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
