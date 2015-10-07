/*--------------------------------------------------------
* DataGridViewStatusCell.cs
* 
* Author: Alexey Nesterov
* Created: 7/29/2009 1:47:20 PM
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
    using AppFramework.Core.Classes.Stock;

    internal class DataGridViewStatusCell : DataGridViewLogCell<LogRecordStatus>
    {
        protected internal override string GetResourceName(LogRecordStatus status)
        {
            string resName = string.Empty;
            switch (status)
            {
                case LogRecordStatus.OK:
                    resName = "tick";
                    break;
                case LogRecordStatus.Warning:
                    resName = "exclamation";
                    break;
                case LogRecordStatus.Error:
                    resName = "cross";
                    break;
                default:
                    break;
            }
            return resName;
        }
    }
}
