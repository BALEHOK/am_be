using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using System.Text.RegularExpressions;
using AppFramework.Core.Helpers;
using System.Web;

namespace AppFramework.Core.PL
{
    public class TextControl : TextBox, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        public TextControl(AssetAttribute attribute)
        {
            this.AssetAttribute = attribute;
            this.TextMode = TextBoxMode.MultiLine;
            this.Text = AssetAttribute.Value;
            Columns = 40;
            Rows = 5;
            this.Attributes["Style"] = "width:" + ApplicationSettings.ControlsWidth;
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            AssetAttribute.Value = Formatting.RemoveScriptTags(this.Text.Trim());
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion

    }
}
