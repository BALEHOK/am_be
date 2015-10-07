using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.PL;

namespace AssetSite.Controls
{
    public partial class GoogleMapsControl : UserControl, IAssetAttributeControl
    {
        private string parseInfoToString()
        {
            return string.Join(";", new string[] { this.SRC_ADDR.Value, this.DST_ADDR.Value, this.additionalInfo.Text });
        }

        private void parseInfoFromString(string source)
        {
            try
            {
                string[] values = source.Split(new char[] { ';' });
                this.SRC_ADDR.Value = values[0];
                this.DST_ADDR.Value = values[1];
                this.additionalInfo.Text = values[2];
            }
            catch
            {
                //something goes wrong
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                parseInfoFromString(AssetAttribute.Value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.SRC_ADDR.Disabled = !this.Editable;
                this.DST_ADDR.Disabled = !this.Editable;
                additionalInfo.ReadOnly = !this.Editable;
            }
        }

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            this.AssetAttribute.Value = parseInfoToString();
            return this.AssetAttribute;
        }

        public bool Editable
        {
            get;
            set;
        }

        public AppFramework.Core.Classes.AssetAttribute AssetAttribute
        {
            get;
            set;
        }

        public void AddAttribute(string name, string value)
        {            
        }
    }
}