using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.Caching
{
    [global::System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    class CacheAttribute : Attribute
    {        
        protected readonly string opName;

        // This is a positional argument
        public CacheAttribute(string name)
        {
            this.opName = name;
        }

        public string OpName
        {
            get { return opName; }
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class CacheValueAttribute : CacheAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValueAttribute"/> class
        /// </summary>
        /// <param name="opName">Name of the caching operation</param>
        public CacheValueAttribute(string name) : base(name)
        {
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class CacheKeyAttribute : CacheAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKeyAttribute"/> class.
        /// </summary>
        /// <param name="opName">Name of the caching operation</param>
        public CacheKeyAttribute(string name) : base(name)
        {
        }
    }
}
