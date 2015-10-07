/*--------------------------------------------------------
* ResourceLoader.cs
* 
* Author: Alexey Nesterov
* Created: 7/29/2009 1:05:42 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace LogTable.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Resources;
    using System.Reflection;

    public sealed class ResourceLoader 
    {
        public ResourceLoader() 
        {
            cache = new Dictionary<string, WeakReference>();
        }
        static ResourceLoader() { }

        private Dictionary<string, WeakReference> cache;

        public static readonly ResourceLoader instance = new ResourceLoader();

        public static ResourceLoader Instance
        {
            get
            {
                return instance;
            }
        }

        public object GetResourceFromAssebmly(string name)
        {
            ResourceManager man = new ResourceManager("LogTable.AppResources", Assembly.GetExecutingAssembly());
            return man.GetObject(name);
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="name">The name of resource.</param>
        /// <returns>Resource object from embedded resources</returns>
        public object GetCachedResource(string name)
        {
            object result = null;

            if (cache.ContainsKey(name))
            {
                WeakReference reference = cache[name];
                if (reference.IsAlive)
                {
                    result = reference.Target;
                }
                else
                {                    
                    result = this.GetResourceFromAssebmly(name);
                    cache[name] = new WeakReference(result);
                }
            }
            else
            {
                result = this.GetResourceFromAssebmly(name);
                cache.Add(name, new WeakReference(result));
            }

            return result;
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Return <see cref="Object"/> value</returns>
        public static object GetResource(string name)
        {
            return ResourceLoader.Instance.GetCachedResource(name);
        }
    }
}