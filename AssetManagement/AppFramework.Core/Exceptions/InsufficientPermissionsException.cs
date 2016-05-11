using System.Web;

namespace AppFramework.Core.Exceptions
{
    public class InsufficientPermissionsException : HttpException
    {
        public InsufficientPermissionsException()
            : base(403, "Insufficient permissions for this action")
        {
        }

        public InsufficientPermissionsException(string message)
            : base(403, message)
        {
        }
    }
}