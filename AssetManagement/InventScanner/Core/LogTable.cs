/*--------------------------------------------------------
* MapTable.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/5/2009 12:13:11 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using AppFramework.Core.Classes.Stock;

    public sealed class LogTable : DataTable
    {
        public LogTable()
        {
            this.Columns.Add("Name", typeof(string));
            this.Columns.Add("Barcode", typeof(string));
            this.Columns.Add("Status", typeof(LogRecordStatus));
            this.Columns.Add("Time", typeof(string));
            this.Columns.Add("AssetTypeId", typeof(long));
            this.Columns.Add("AssetId", typeof(long));
            this.Columns.Add("Message", typeof(string));
            this.Columns.Add("CurrentLocation", typeof(string));
            this.Columns.Add("CurrentLocationId", typeof(long));
        }
    }
}