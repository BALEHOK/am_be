/*--------------------------------------------------------
* AssetsTable.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/5/2009 1:28:33 PM
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

    [Serializable]
    public sealed class AssetsTable : DataTable
    {
        public AssetsTable()
        {
            this.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn("Name", typeof(string)),
                new DataColumn("Barcode", typeof(string)),
                new DataColumn("AssetType", typeof(string)),
                new DataColumn("Owner", typeof(string)),
                new DataColumn("AssetId", typeof(long)),
                new DataColumn("AssetTypeId", typeof(long))
            });
        }
    }
}
