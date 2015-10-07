using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE
{
    /// <summary>
    /// Holds the information about status of any Import/export operation
    /// </summary>
    public class StatusInfo
    {
        /// <summary>
        /// Warning messages.
        /// </summary>
        public List<string> Warnings { get; private set; }
        
        /// <summary>
        /// Error messages.
        /// </summary>
        public List<string> Errors { get; private set; }
        
        /// <summary>
        /// Status of performed operation.
        /// </summary>
        public bool IsSuccess { get; set; }

        public StatusInfo() 
        {
            Warnings = new List<string>();
            Errors = new List<string>();
            IsSuccess = true;
        }

        /// <summary>
        /// Aggregates different statuses.
        /// </summary>
        /// <param name="anotherInfo"></param>
        public void Add(StatusInfo anotherStatus)
        {
            this.IsSuccess &= anotherStatus.IsSuccess;
            this.Warnings.AddRange(anotherStatus.Warnings);
            this.Errors.AddRange(anotherStatus.Errors);
        }
    }
}
