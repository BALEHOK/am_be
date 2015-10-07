/*--------------------------------------------------------
* DataGridViewDirectionColumn.cs
* 
* Author: Alexey Nesterov
* Created: 7/29/2009 1:54:00 PM
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

    public class DataGridViewDirectionColumn : DataGridViewColumn
    {
        public DataGridViewDirectionColumn()
            : base(new DataGridViewDirectionCell())
        {
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }
}
