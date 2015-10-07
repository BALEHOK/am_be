/*--------------------------------------------------------
* InventLogTable.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/3/2009 3:37:15 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace LogTable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using AppFramework.Core.Classes.Stock;

    public class InventLogTable : DataTable
    {
        public InventLogTable()
        {
            this.Columns.Add("Name", typeof(string));
            this.Columns.Add("Barcode", typeof(string));
            this.Columns.Add("Status", typeof(LogRecordStatus));
            this.Columns.Add("Time", typeof(string));
        }
    }
}
