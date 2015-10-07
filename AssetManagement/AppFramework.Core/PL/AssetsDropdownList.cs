using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Properties;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL
{
    public class AssetsDropdownList : DropDownList, IAssetAttributeControl
    {
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }

        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        private List<KeyValuePair<long, string>> _assets;

        public AssetsDropdownList(AssetAttribute attribute)
        {
            this.AssetAttribute = attribute;
            this.Attributes["Style"] = "width:" + ApplicationSettings.ControlsWidth;

            AssetTypeAttribute attrConf = attribute.GetConfiguration();

            if (attrConf.RelatedAssetTypeAttributeID == 0 || attrConf.RelatedAssetTypeID == 0)
            {
                return;
            }

            // get all list of assets  
            this._assets = AssetsService.GetIdNameListByAssetType(
                AssetTypeRepository.GetById((long)attrConf.RelatedAssetTypeID)).ToList();

            // add empty value
            this.Items.Add(new ListItem(Resources.SelectText, string.Empty));

            foreach (KeyValuePair<long, string> asset in this._assets)
            {
                // Value - ID
                this.Items.Add(new ListItem(asset.Value, asset.Key.ToString()));
            }

            if (this.Items.FindByValue(attribute.ValueAsId.ToString()) != null)
                this.SelectedValue = attribute.ValueAsId.ToString();

            this.Load += new EventHandler(AssetsDropdownList_Load);

        }

        public void AssetsDropdownList_Load(Object sender, EventArgs e)
        {
            this.Enabled = this.AssetAttribute.GetConfiguration().DataType.Editable && this.Editable;
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            if (this.SelectedValue != string.Empty)
            {
                long id = 0;
                long.TryParse(this.SelectedValue, out id);
                this.AssetAttribute.ValueAsId = id;
            }
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion
    }
}
