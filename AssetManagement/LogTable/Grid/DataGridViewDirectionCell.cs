/*--------------------------------------------------------
* DataGridViewDirectionCell.cs
* 
* Author: Alexey Nesterov
* Created: 7/29/2009 1:50:27 PM
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

    internal class DataGridViewDirectionCell : DataGridViewLogCell<TransactionTypeCode>
    {
        protected internal override string GetResourceName(TransactionTypeCode status)
        {
            string resName = string.Empty;
            switch (status)
            {
                case TransactionTypeCode.In:
                    resName = "arrow_in";
                    break;
                case TransactionTypeCode.Out:
                    resName = "arrow_out";
                    break;
                case TransactionTypeCode.Move:
                    resName = "arrow_move";
                    break;
                default:
                    break;
            }
            return resName;
        }
    }
}
