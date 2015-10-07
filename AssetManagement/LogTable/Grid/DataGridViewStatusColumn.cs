/*--------------------------------------------------------
* DataGridViewStatusColumn.cs
* 
* Author: Alexey Nesterov
* Created: 7/29/2009 12:27:46 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace LogTable.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    public class DataGridViewStatusColumn : DataGridViewColumn
    {
        public DataGridViewStatusColumn()
            : base(new DataGridViewStatusCell())
        {
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }
}
