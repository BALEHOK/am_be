using System;
using System.Web.UI;

namespace AssetSite.Controls
{
    public partial class DynListItemEdit : UserControl
    {
        public long DynListUID { get; set; }
        public long DynListItemId { get; set; }
        public string DynListItemValue { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            lbtnItemName.InnerText = this.DynListItemValue;
            lbtnRemoveItem.Attributes.Add("onclick", "return OnDelete(" + 
                this.DynListItemId + ",'" + DynListItemDataContainer.ClientID + "');");
        }
    }
}