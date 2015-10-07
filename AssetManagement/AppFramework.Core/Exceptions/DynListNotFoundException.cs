using System;

namespace AppFramework.Core.Exceptions
{
    public class DynListNotFoundException : EntityNotFoundException
    {
        public DynListNotFoundException()
            : base()
        {
        }
        public DynListNotFoundException(string message)
            : base(message)
        {
        }
    }
}
