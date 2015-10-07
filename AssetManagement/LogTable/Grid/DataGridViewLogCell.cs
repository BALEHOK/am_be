/*--------------------------------------------------------
* DataGridViewLogCell.cs
* 
* Author: Alexey Nesterov
* Created: 7/29/2009 12:33:25 PM
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
    using AppFramework.Core.Classes.Stock;
    using System.Drawing;

    public abstract class DataGridViewLogCell<T> : DataGridViewImageCell
    {
        public DataGridViewLogCell()
            : base()
        {
            
        }

        protected abstract internal string GetResourceName(T status);        

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {            
            T status = default(T);
            if ((value != null) && (value is T || value is int))
            {
                status = (T)value;
            }
            else
            {
                throw new ArgumentException("Wrong value type for cell. Must be LogRecordStatus");
            }

            Image statusImg = null;
            string resName = this.GetResourceName(status);
           
            statusImg = ResourceLoader.GetResource(resName) as Image;
            
            return statusImg;
        }
    }
}
