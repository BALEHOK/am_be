using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE
{
    /// <summary>
    /// Holds the results of any Import/Export operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TransferResult<T>
    {
        public StatusInfo Status { get; set; }
        public IEnumerable<T> DataSet { get; set; }

        public TransferResult() { }

        public TransferResult(StatusInfo status, IEnumerable<T> data)
            : base()
        {
            Status = status;
            DataSet = data;
        }
    }
}
