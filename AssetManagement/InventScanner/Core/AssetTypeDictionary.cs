/*--------------------------------------------------------
* AssetTypeDictionary.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/6/2009 1:22:21 PM
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
    using AppFramework.Core.Classes;

    [Serializable]
    public sealed class AssetTypeDictionary : SerializableDictionary<long, AssetType> 
    {

    }
}
