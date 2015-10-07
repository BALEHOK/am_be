using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.Caching
{
    /// <summary>
    /// Create Cache instances
    /// </summary>
    public sealed class CacheFactory
    {
        /// <summary>
        /// Gets the cache for class T instances.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="opName">Name of the operation.</param>
        /// <returns></returns>
        public static Cache<T> GetCache<T>(string opName) where T : class
        {
            return new Cache<T>(opName);
        }
    }
}
