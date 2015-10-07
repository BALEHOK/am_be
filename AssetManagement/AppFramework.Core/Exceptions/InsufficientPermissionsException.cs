using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Exceptions
{
    public class InsufficientPermissionsException : Exception
    {
        public InsufficientPermissionsException()
            : base("Insufficient permissions for this action")
        {

        }

        public InsufficientPermissionsException(string message)
            : base(message)
        {

        }
    }
}
