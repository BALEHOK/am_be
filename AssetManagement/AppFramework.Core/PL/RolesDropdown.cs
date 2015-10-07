using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL
{
    public class RolesDropdown : UserControl, IAssetAttributeControl
    {
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }

        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            _dropdown.Attributes.Add(name, value);
        }

        private readonly DropDownList _dropdown = new DropDownList
        {
            DataTextField = "RoleName",
            DataValueField = "RoleId",
            ID = "RolesDD"
        };

        public RolesDropdown(AssetAttribute attribute)
        {
            AssetAttribute = attribute;
        }

        void Dropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyAssetValidation(_dropdown.SelectedValue);
        }

        private void ModifyAssetValidation(string assetAttributeValue)
        {
            //Email and Password are not required if user is 'Only Person'
            if (!String.IsNullOrEmpty(assetAttributeValue))
            {
                bool isPasswordAndEmailRequired = int.Parse(assetAttributeValue) != (int)PredefinedRoles.OnlyPerson;
                foreach (var parentAssetAttribute in AssetAttribute.ParentAsset.GetConfiguration().Attributes)
                {
                    if (parentAssetAttribute.DBTableFieldName == AttributeNames.Password ||
                        parentAssetAttribute.DBTableFieldName == AttributeNames.Email)
                    {
                        parentAssetAttribute.IsRequired = isPasswordAndEmailRequired;
                    }
                }
            }
        }


        protected override void OnInit(EventArgs e)
        {
            this.Controls.Clear();

            if (Editable)
            {
                InitDropdown();
                this.Controls.Add(_dropdown);

                _dropdown.SelectedIndexChanged += Dropdown_SelectedIndexChanged;

                if (!string.IsNullOrEmpty(AssetAttribute.Value))
                    ModifyAssetValidation(AssetAttribute.Data.Value.ToString());
            }
            else
            {
                this.Controls.Add(GetLabel(AssetAttribute.Value));
            }

            base.OnInit(e);
        }

        private void InitDropdown()
        {
            _dropdown.DataSource = UnitOfWork.RoleRepository.Get();
            _dropdown.Attributes.Add("onchange", "OnRolesComboBoxIndexChanged(this)");
            _dropdown.DataBind();

            if (_dropdown.Items.FindByValue(AssetAttribute.Data.Value.ToString()) != null)
                _dropdown.SelectedValue = AssetAttribute.Data.Value.ToString();
        }

        /// <summary>
        /// In case if control will be used in grid
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            GridViewRow li = this.NamingContainer as GridViewRow;
            Asset a = li.DataItem as Asset;

            if (a == null)
                throw new NullReferenceException("Cannot bind asset to AssetsGrid row");

            Object value = DataBinder.Eval(li.DataItem, string.Format("[{0}].Value", AssetAttribute.GetConfiguration().DBTableFieldName));
            if (value != null)
            {
                this.Controls.Clear();
                this.Controls.Add(GetLabel(value.ToString()));
            }
        }

        private WebControl GetLabel(string text)
        {
            return new Label() { Text = text };
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Editable)
            {
                string scriptKey = "Roles";
                if (!Page.ClientScript.IsClientScriptIncludeRegistered(scriptKey))
                {
                    Page.ClientScript.RegisterClientScriptInclude(scriptKey, Page.ResolveUrl("~/javascript/Roles.js"));
                    var startupScript =
                        String.Format("<script>$(function() {{ OnRolesComboBoxIndexChanged('#{0}'); }})</script>",
                                      _dropdown.ClientID);
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), scriptKey, startupScript);
                }
            }
        }

        #region IAssetAttributeControl Members

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            if (Editable)
            {
                AssetAttribute.Value = _dropdown.SelectedValue;
            }
            return AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion
    }
}
