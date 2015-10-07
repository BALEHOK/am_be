using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppFramework.Core.PL.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class FormElementContainer: Panel, INamingContainer
    {
        private FormElementContainerType containerType;

        /// <summary>
        /// Container Constructor
        /// </summary>
        public FormElementContainer(FormElementContainerType containerType)
        {
            this.containerType = containerType;
        }

        /// <summary>
        /// 
        /// </summary>
        public FormElementContainerType ContainerType
        {
            get { return containerType; }
        }
    }
}
