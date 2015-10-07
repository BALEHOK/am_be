using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL
{
    public class AssetAttributeLabel : Label, IAssetAttributeControl
    {
        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        public AssetAttribute AssetAttribute { get; set; }
        
        public AssetAttributeLabel(AssetAttribute attribute)
        {
            AssetAttribute = attribute;
        }

        public string ForcedText
        {
            get;
            set;
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = string.IsNullOrEmpty(this.ForcedText) ? value : this.ForcedText;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (AssetAttribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.Email && !Editable)
            {
                Text = string.Format("<a href='mailto:{0}'>{0}</a>", AssetAttribute.Value);
            }
            else
            {
                this.Text = AssetAttribute.Value;
            }
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            GridViewRow li = this.NamingContainer as GridViewRow;
            Asset a = li.DataItem as Asset;

            if (a != null)
            {
                //throw new NullReferenceException("Cannot bind asset to AssetsGrid row");


                Object value = DataBinder.Eval(li.DataItem, string.Format("[{0}].Value", AssetAttribute.GetConfiguration().DBTableFieldName));
                if (value != null)
                {
                    this.Text = value.ToString();
                }
            }
        }
    }
}
