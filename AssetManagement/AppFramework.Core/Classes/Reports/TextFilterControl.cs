namespace AppFramework.Core.Classes.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

    public class TextFilterControl : TextBox, IFilterControl
    {
        #region IFilterControl Members
        public string GetValue()
        {
            return this.Text;
        }

        public string GetText()
        {
            return this.Text;
        }

        #endregion
    }
}
