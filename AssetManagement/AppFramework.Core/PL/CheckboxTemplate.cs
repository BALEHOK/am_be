using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppFramework.Core.PL
{
    public class CheckboxTemplate : ITemplate
    {

        public CheckboxTemplate() { }

        #region ITemplate Members

        public void InstantiateIn(Control container)
        {
            CheckBox chk = new CheckBox();
            container.Controls.Add(chk);
        }

        #endregion
    }
}
