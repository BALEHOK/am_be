using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.PL;
using AssetSite.Helpers;

namespace AssetSite.Controls
{
    public partial class Calendar : UserControl, IAssetAttributeControl
    {
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {            
        }

        public bool Editable { get; set; }

        protected string Locale
        {
            get
            {
                return CookieWrapper.Language.Split(new char[] { '-' })[0].ToLower();
            }
        }

        protected string DatePattern
        {
            get
            {
                if (ApplicationSettings.DisplayCultureInfo.TwoLetterISOLanguageName == "en")
                {
                    return "mm/dd/yy";
                }
                else if (ApplicationSettings.DisplayCultureInfo.TwoLetterISOLanguageName == "fr")
                {
                    return "dd/mm/yy";
                }
                else
                {
                    return "dd-mm-yy";
                }
            }
        }

        protected string ScriptExtension = string.Empty;

        private DateTimeFormatInfo _displayFormat = ApplicationSettings.DisplayCultureInfo.DateTimeFormat;
        private bool isBinded = false;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (isBinded)
                return;

            bool isUpdateDateField
                = AssetAttribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.CurrentDate;

            txtDateTime.Visible = Editable && !isUpdateDateField;
            lblDateTime.Visible = !txtDateTime.Visible;

            DateTime? dt = getDateTime();

            if (Editable && !isUpdateDateField)
            {
                if (Locale != "en")
                    Page.ClientScript.RegisterClientScriptInclude("datepicker_localization",
                      string.Format("/javascript/jquery.ui.datepicker-{0}.js", Locale));

                string setDateScript = string.Empty;
                if (dt.HasValue)
                    ScriptExtension = @"$('#" + this.txtDateTime.ClientID +
                        "').datepicker('setDate', new Date(" + dt.Value.Year + "," + (dt.Value.Month - 1) + "," + dt.Value.Day + "));";
            }
            else
            {
                if (dt.HasValue)
                    lblDateTime.Text = dt.Value.ToString(_displayFormat);
                else if (isUpdateDateField)
                    lblDateTime.Text = DateTime.Now.ToString(_displayFormat);
            }
        }

        public AppFramework.Core.Classes.AssetAttribute GetAttribute()
        {
            if (Editable)
            {
                if (AssetAttribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.CurrentDate)
                {
                    AssetAttribute.Value = DateTime.Now.ToString(_displayFormat);
                }
                else
                {
                    AssetAttribute.Value = txtDateTime.Text;
                }
            }
            return AssetAttribute;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            isBinded = true;
            GridViewRow li = this.NamingContainer as GridViewRow;
            if (li.DataItem != null)
            {
                Object value = DataBinder.Eval(li.DataItem, string.Format("[{0}].Value", AssetAttribute.GetConfiguration().DBTableFieldName));
                if (value != null)
                {
                    DateTime dt;
                    if (DateTime.TryParse(value.ToString(), _displayFormat, DateTimeStyles.AllowWhiteSpaces, out dt))
                        lblDateTime.Text = dt.ToString(_displayFormat);
                }
            }
        }

        private DateTime? getDateTime()
        {
            DateTime dt;
            if (!DateTime.TryParse(AssetAttribute.Value, _displayFormat, DateTimeStyles.AllowWhiteSpaces, out dt))
                return null;
            return dt;
        }
    }
}