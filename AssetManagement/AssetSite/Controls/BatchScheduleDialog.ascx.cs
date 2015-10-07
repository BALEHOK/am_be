using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;
using AssetSite.Helpers;
using AppFramework.Core.Classes;

namespace AssetSite.Controls
{
    public partial class BatchScheduleDialog : System.Web.UI.UserControl
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptInclude(this, typeof(BatchScheduleDialog), "timepicker", "/javascript/jquery.ui.timepicker.js");
            if (Locale != "en")
            {
                Page.ClientScript.RegisterClientScriptInclude("datepicker_localization",
                  string.Format("/javascript/jquery.ui.datepicker-{0}.js", Locale));
                Page.ClientScript.RegisterClientScriptInclude("timepicker_localization",
                  string.Format("/javascript/jquery.ui.timepicker-{0}.js", Locale));
            }

            foreach (BatchActionType t in EnumToList<BatchActionType>())
            {
                string description = GetEnumDescription(t);
                if (!string.IsNullOrEmpty(description))
                    dlBatchAction.Items.Add(new ListItem(description, ((int)t).ToString()));
            }
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return string.Empty;
        }

        private static IEnumerable<T> EnumToList<T>()
        {
            Type enumType = typeof(T);
            // Can't use generic type constraints on value types,    
            // so have to do check like this    
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");
            Array enumValArray = Enum.GetValues(enumType);
            List<T> enumValList = new List<T>(enumValArray.Length);
            foreach (int val in enumValArray)
            {
                enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
            }
            return enumValList;
        }
    }
}