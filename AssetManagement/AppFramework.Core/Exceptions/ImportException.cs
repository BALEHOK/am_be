using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Exceptions
{
    public class ImportException : Exception
    {
        public ImportException(string message)
            : base(message)
        {
            
        }
    }
}
