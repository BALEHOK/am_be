using System;

namespace AppFramework.Core.Exceptions
{
    class FSObjectCreationException : Exception
    {
        public FSObjectCreationException(string fileObjName)
            : base(string.Format("Cannot create FileSystem object: '{0}'", fileObjName)) { }
    }
}
