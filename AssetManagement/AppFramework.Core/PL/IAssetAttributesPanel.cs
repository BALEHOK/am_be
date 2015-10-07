using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL
{
    public interface IAssetAttributesPanel
    {
        /// <summary>
        /// Returns the list of attributes with values assigned to this panel
        /// </summary>
        /// <returns>List of AssetAttribute</returns>
        List<AssetAttribute> GetAttributes();
    }
}
