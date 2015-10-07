using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes;

namespace AppFramework.Core.Classes
{
    public class AssetsComparer : IEqualityComparer<Asset>
    {
        public AssetsComparer()
        {
           
        }        

        public bool Equals(Asset x, Asset y)
        {       
            if (x["DynEntityUid"].Value == y["DynEntityUid"].Value)
            {
                return true;
            }
            else return false;
        }

        public int GetHashCode(Asset asset)
        {
            return asset.ToString().ToLower().GetHashCode();
        }

    }    
}
