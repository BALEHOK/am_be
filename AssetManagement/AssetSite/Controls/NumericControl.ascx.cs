using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.PL;

namespace AssetSite.Controls
{
    public partial class NumericControl : UserControl, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            txtNumeric.Attributes.Add(name, value);
            lblNumeric.Attributes.Add(name, value);
        }

        public bool Editable { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtNumeric.Visible = Editable;
            lblNumeric.Visible = !Editable;
           
            if (!IsPostBack)
            {
                //Page.ClientScript.RegisterClientScriptInclude("hint", "/javascript/jquery.hint.js");                
                //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_hint_" + this.ClientID,
                //    "$(function () { $('#" + txtNumeric.ClientID + "').hint(); });", true);

                string separator = ApplicationSettings.DisplayCultureInfo.NumberFormat.CurrencyDecimalSeparator;
                var value = AssetAttribute.Value;

                switch (AssetAttribute.GetConfiguration().DataTypeEnum)
                {
                    case Enumerators.DataType.Float:
                    case Enumerators.DataType.Euro:
                    case Enumerators.DataType.Money:
                    case Enumerators.DataType.USD:
                        txtNumeric.Attributes.Add("title", string.Format("####{0}##", separator));                        
                        if (!string.IsNullOrEmpty(value))
                        {
                            var parts = value.Split(new string[] { separator }, StringSplitOptions.None);
                            if (parts.Count() == 1)
                            {
                                value = string.Format("{0}{1}{2}", value, separator, "00");
                            }
                            else if (parts.Count() == 2)
                            {
                                value = string.Format("{0}{1}{2}", parts[0], separator, parts[1].Length == 1 ? parts[1] + "0" : parts[1]);
                            }
                            else
                            {
                                throw new FormatException(string.Format("{0} does not recognized as a valid numeric value.", value));
                            }
                        }
                        break;
                }

                lblNumeric.Text = string.Format("{0}{1}", getMoneySign(), value);
                txtNumeric.Text = value;
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            GridViewRow li = this.NamingContainer as GridViewRow;
            var a = li.DataItem as AppFramework.Core.Classes.Asset;

            if (a != null)
            {
                Object value = DataBinder.Eval(li.DataItem, string.Format("[{0}].Value", AssetAttribute.GetConfiguration().DBTableFieldName));
                if (value != null)
                {
                    lblNumeric.Text = value.ToString();
                }
            }
        }

        private string getMoneySign()
        {
            Enumerators.DataType datatype = AssetAttribute.GetConfiguration().DataTypeEnum;
            if (datatype == Enumerators.DataType.Euro)
            {
                return "&euro;&nbsp;";
            }
            else if (datatype == Enumerators.DataType.USD)
            {
                return "$&nbsp;";
            }
            else if (datatype == Enumerators.DataType.Money)
            {
                //Just for now, will discuss in future
                //return "&curren;&nbsp;";
                return "&euro;&nbsp;";
            }
            else return string.Empty;
        }

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            if (Editable)
            {
                AssetAttribute.Value = txtNumeric.Text;
            }
            return AssetAttribute;
        }
    }
}