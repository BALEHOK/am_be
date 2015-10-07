using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    internal interface IIntegrityValidator<T>
    {
        bool Validate(T obj);
        bool IsNew { get; }
        bool AllowErrorsCorrection { get; set; }
    }
}
