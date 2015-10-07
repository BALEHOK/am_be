using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Exceptions
{
    public class FieldLinkingException : ImportException
    {
        public FieldLinkingException(string message)
            : base(message)
        {
            
        }
    }
}
