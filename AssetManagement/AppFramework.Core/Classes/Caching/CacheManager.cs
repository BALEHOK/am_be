using System.Linq;

namespace AppFramework.Core.Classes.Caching
{
    using Microsoft.Practices.EnterpriseLibrary.Caching;
    using System;
    using System.Reflection;
    using System.Web;

    /// <summary>
    /// Core of caching system, provide access to base caching method - <![CDATA[Get<T>()]]>
    /// </summary>
    /// <typeparam name="T">Class which instances will store in cache</typeparam>
    public class Cache<T> : ICache<T> where T : class
    {
        private string operationName;
        private Type type;
        private string keyPrefix;

        /// <summary>
        /// Gets the object count.
        /// </summary>
        /// <value>The object count.</value>
        public static long ObjCount
        {
            get
            {
                long cnt = 0;
                CacheManager cacheManager = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager() as CacheManager;
                if (cacheManager != null)
                {
                    cnt = cacheManager.Count;
                }
                return cnt;
            }
        }

        /// <summary>
        /// Gets the memory usage.
        /// </summary>
        /// <value>The memory usage.</value>
        public static int MemUsage
        {
            get
            {
                int mem = 0;
                CacheManager cacheManager = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager() as CacheManager;
                if (cacheManager != null)
                {
                    mem = 0;// sizeof(cacheManager);
                }

                return mem;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="opName">Name of the operation</param>
        public Cache(string opName)
        {
            if (HttpContext.Current != null)
            {
                keyPrefix = HttpContext.Current.User.Identity.Name;
            }
            else
            {
                keyPrefix = string.Empty;
            }

            operationName = opName;
            type = typeof(T);
        }

        /// <summary>
        /// Finds the method of class T which generates value for caching
        /// </summary>
        /// <returns></returns>
        public MethodInfo FindGetValueMethod()
        {
            return type.GetMethods()
                .Select(m => new Tuple<MethodInfo, CacheValueAttribute>(m,
                   m.GetCustomAttributes(typeof(CacheValueAttribute), false)
                    .SingleOrDefault() as CacheValueAttribute))
                .Where(m => m.Item2 != null && m.Item2.OpName == operationName)
                .Select(m => m.Item1)
                .SingleOrDefault();
        }

        /// <summary>
        /// Finds the method of class T which generates key for caching
        /// </summary>
        /// <returns></returns>
        public MethodInfo FindGetKeyMethod()
        {
            MethodInfo getKey = null;
            foreach (MethodInfo method in type.GetMethods())
            {               
                CacheKeyAttribute attr2 = Attribute.GetCustomAttribute(method, typeof(CacheKeyAttribute)) as CacheKeyAttribute;
                if (attr2 != null && attr2.OpName == operationName)
                {
                    if (getKey == null)
                    {
                        getKey = method;
                    }
                    else
                    {
                        throw new Exception(string.Format("Two static methods have CacheKey attribute with same operation name in class {0}",
                            type.ToString()));
                    }
                }
            }

            return getKey;
        }

        /// <summary>
        /// Gets value from cache or generate new one using static methods of T class
        /// </summary>
        /// <typeparam name="T">Class of cache stored value</typeparam>        
        /// <param name="arg">Argument list</param>
        /// <returns></returns>
        public T Get(params object[] arg)
        {
            T value = null;
            MethodInfo getValue = null, getKey = null;

            #region Trying to find methods for generation cache key and value
            getValue = this.FindGetValueMethod();
            getKey = this.FindGetKeyMethod();

            if (getKey == null)
            {
                throw new Exception(string.Format("{0} class not contain static method marked with CacheKey attribute with operation name {1}", type.ToString(), operationName));
            }

            if (getValue == null)
            {
                throw new Exception(string.Format("{0} class not contain static method marked with CacheValue attribute with operation name {1}", type.ToString(), operationName));
            }
            #endregion

            string key = getKey.Invoke(null, arg).ToString() + keyPrefix;       // get key

            var cacheManager = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager() as CacheManager;
            if (cacheManager != null)
            {
                if (cacheManager.Contains(key)) // if value for key found in cache
                {
                    object temp = cacheManager.GetData(key); // find out type of stored value
                    if (temp is T || temp == null) // is stored not null and type T - ok, return value from cache
                    {
                        value = temp as T;
                    }
                    else // value not null and type not equal to T
                    {
                        throw new Exception(string.Format("Invalid cache value - extected type {0}, actual {1}",
                                                          type.ToString(), temp.GetType().ToString()));
                    }
                }
                else // value not found in cache
                {
                    value = getValue.Invoke(null, arg) as T; // generate new one
                    cacheManager.Add(key, value); // and add to cache
                }
            }

            return value;
        }

        /// <summary>
        /// Adds the instance to cache. Previous value will removed
        /// </summary>
        /// <param name="args">The args.</param>
        public void Add(object value, params object[] args)
        {
            MethodInfo getKey = this.FindGetKeyMethod();
            MethodInfo getValue = this.FindGetValueMethod();
            
            CacheManager cacheManager = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager() as CacheManager;
            if (cacheManager != null)
            {
                string key = getKey.Invoke(null, args).ToString() + keyPrefix;
                cacheManager.Add(key, value);
            }
        }

        /// <summary>
        /// Removes the item from cache
        /// </summary>
        /// <param name="arg">The arg.</param>
        public void Remove(params object[] arg)
        {
            MethodInfo getKey = this.FindGetKeyMethod();
            string key = getKey.Invoke(null, arg).ToString() + keyPrefix;

            CacheManager cacheManager = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager() as CacheManager;
            if (cacheManager != null)
            {
                cacheManager.Remove(key);
            }
        }

        /// <summary>
        /// Flushes cache - for all type of objects
        /// </summary>
        public static void Flush()
        {
            CacheManager cacheManager = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager() as CacheManager;
            if (cacheManager != null)
            {
                cacheManager.Flush();
            }
        }        
    }
}