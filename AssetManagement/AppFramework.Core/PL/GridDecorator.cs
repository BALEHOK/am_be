using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace AppFramework.Core.PL
{
    /// <summary>
    /// Describes decorator for grid element
    /// </summary>
    abstract class GridDecorator : GridView
    {
        protected GridView decoratedGrid;

        public GridDecorator(GridView decoratedGrid)
        {
            this.decoratedGrid = decoratedGrid;
        }
    }
}
