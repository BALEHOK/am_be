using Microsoft.Practices.Unity;

namespace AppFramework.Core.Interfaces
{
    public interface IHttpUnityApplication
    {
        IUnityContainer UnityContainer { get; }
    }

}
