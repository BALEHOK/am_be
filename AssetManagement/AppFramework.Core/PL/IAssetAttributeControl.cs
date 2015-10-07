using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace AppFramework.Core.PL
{
    public interface IAssetAttributeControl
    {
        /// <summary>
        /// Each control must return the value of attribute which it represents
        /// </summary>
        /// <returns>AssetAttribute value</returns>
        AssetAttribute GetAttribute();

        /// <summary>
        /// Can the value of this control be modified by user or not
        /// </summary>
        bool Editable { get; set; }

        /// <summary>
        /// Gets or sets the AssetAttribute
        /// </summary>
        AssetAttribute AssetAttribute { get; set; }        

        void AddAttribute(string name, string value);
    }
}