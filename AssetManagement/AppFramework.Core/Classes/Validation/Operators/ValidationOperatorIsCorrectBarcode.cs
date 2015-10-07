/*--------------------------------------------------------
* ValidationOperatorIsCorrectBarcode.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/12/2009 3:03:15 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

using AppFramework.Core.Validation;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.Classes.Validation.Operators
{
    using AppFramework.Core.Classes.Barcode;

    public class ValidationOperatorIsCorrectBarcode : ValidationOperatorBase
    {
        [Dependency]
        public IBarcodeProvider BarcodeProvider
        {
            get { return _barcodeProvider ?? (_barcodeProvider = new DefaultBarcodeProvider()); }
            set
            {
                if (value != null && value != _barcodeProvider)
                    _barcodeProvider = value;
            }
        }

        private IBarcodeProvider _barcodeProvider;

        public override ValidationResultLine Validate(string value)
        {
            string barcode = value;
            var res = new ValidationResultLine(string.Empty);
            if (!string.IsNullOrEmpty(barcode))
            {
                bool valid = BarcodeProvider.ValidateBarcode(barcode);
                res.IsValid = valid;
                if (!valid)
                {
                    res.Message = "Given barcode is not valid";
                }
            }
            else
            {
                res.IsValid = true;
            }
            return res;
        }
    }
}
