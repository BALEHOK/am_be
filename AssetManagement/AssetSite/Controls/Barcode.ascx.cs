using System;
using System.Web.UI;
using AppFramework.Core.Classes;
using AppFramework.Core.PL;

namespace AssetSite.Controls
{
    public partial class Barcode : UserControl, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            txtBarcode.Attributes.Add(name, value);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Editable)
            {
                if (!IsPostBack)
                {
                    txtBarcode.Text = AssetAttribute.Value;
                }

                mv1.ActiveViewIndex = 1;
                btnGenerate.Attributes.Add("onclick", string.Format("GenerateBarcode('{0}'); return false;", txtBarcode.ClientID));
            }
            else
            {
                mv1.ActiveViewIndex = 0;
                imgBarCode.ImageUrl = string.Format("http://{0}:{1}/barcodeImage.aspx?barcode={2}",
                                                    Request.Url.DnsSafeHost,
                                                    Request.Url.Port,
                                                    AssetAttribute.Value);
            }
        }

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            if (Editable)
            {
                AssetAttribute.Value = txtBarcode.Text;
            }
            return AssetAttribute;
        }

        public bool Editable { get; set; }
    }
}