/*--------------------------------------------------------
* InMemoryStorageContainer.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 7/30/2009 2:45:42 PM
* Purpose: Singleton pattern realization for storing information in memory.
* 
* Revisions:
* -------------------------------------------------------*/

namespace AppFramework.Core.AC.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public sealed class InMemoryStorageContainer
    {
        # region Singleton implementation
        private static readonly InMemoryStorageContainer obj = new InMemoryStorageContainer();

        private InMemoryStorageContainer() { }

        static InMemoryStorageContainer() { }

        private object _data;

        public object Data
        {
            get
            {
                return this._data;
            }
            set
            {
                lock (this)
                {
                    this._data = value;
                }
            }
        }

        public static InMemoryStorageContainer Instance
        {
            get
            {
                return obj;
            }
        }
        #endregion
    }
}
