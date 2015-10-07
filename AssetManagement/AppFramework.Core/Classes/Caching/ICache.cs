using System.Reflection;

namespace AppFramework.Core.Classes.Caching
{
    public interface ICache<T> where T : class
    {
        /// <summary>
        /// Finds the method of class T which generates value for caching
        /// </summary>
        /// <returns></returns>
        MethodInfo FindGetValueMethod();

        /// <summary>
        /// Finds the method of class T which generates key for caching
        /// </summary>
        /// <returns></returns>
        MethodInfo FindGetKeyMethod();

        /// <summary>
        /// Gets value from cache or generate new one using static methods of T class
        /// </summary>
        /// <typeparam name="T">Class of cache stored value</typeparam>        
        /// <param name="arg">Argument list</param>
        /// <returns></returns>
        T Get(params object[] arg);

        /// <summary>
        /// Adds the instance to cache. Previous value will removed
        /// </summary>
        /// <param name="args">The args.</param>
        void Add(object value, params object[] args);

        /// <summary>
        /// Removes the item from cache
        /// </summary>
        /// <param name="arg">The arg.</param>
        void Remove(params object[] arg);
    }
}