using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace AppFramework.Core.PL
{
    public class EmptyTemplate : ITemplate
    {

        public List<Control> Controls
        {
            get
            {
                return this._controls;
            }
        }


        private List<Control> _controls = new List<Control>();

        public EmptyTemplate() { }     

        public void InstantiateIn(Control container) 
        {
            foreach (Control ctrl in this.Controls)
            {             
                container.Controls.Add(ctrl);  
            }            
        }
    }
}
