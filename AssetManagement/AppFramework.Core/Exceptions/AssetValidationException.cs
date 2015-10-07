using AppFramework.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Core.Exceptions
{
    public class AssetValidationException : Exception
    {
        public ValidationResult ValidationResult { get; private set; }

        public AssetValidationException(ValidationResult validationResult)
        {
            this.ValidationResult = validationResult;
        }
    }
}
