using System;

namespace AppFramework.Core.Exceptions
{
    public class AssetNotFoundException : EntityNotFoundException
    {
        public AssetNotFoundException()
            : base()
        {
        }
        public AssetNotFoundException(string message)
            : base(message)
        {
        }
    }
}
