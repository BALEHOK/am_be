using System.Web.UI.WebControls;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL
{
    public class UrlControl : HyperLink, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        public UrlControl(AssetAttribute attribute)
        {
            this.AssetAttribute = attribute;
            this.Attributes["Style"] = "width:" + ApplicationSettings.ControlsWidth;
            this.Text = attribute.Value;
            this.NavigateUrl = attribute.Value;
            this.Target = "_blank";
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            this.AssetAttribute.Value = this.NavigateUrl;
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion
    }
}
