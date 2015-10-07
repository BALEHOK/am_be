using System;

namespace AppFramework.Core.Exceptions
{
    public class AssetTypeNotFoundException : EntityNotFoundException
    {
        public AssetTypeNotFoundException()
            : base()
        {
        }
        public AssetTypeNotFoundException(string message)
            : base(message)
        {
        }
    }
}
