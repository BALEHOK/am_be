/*--------------------------------------------------------
* KeyValuePairs.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/5/2009 6:15:27 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public struct KeyValue<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public KeyValue(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    public sealed class KeyValuePairs : List<KeyValue<long, string>>
    {
        internal KeyValue<long, string> FindByKey(long assetTypeUid)
        {
            return this.FirstOrDefault(p => p.Key == assetTypeUid);
        }
    }
}
