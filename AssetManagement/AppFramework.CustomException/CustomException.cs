using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.CustomException
{
    public class CustomException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class.
        /// </summary>
        public CustomException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CustomException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public CustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the error message to the log and via email.
        /// </summary>
        public void WriteLogAndEmail()
        {            
            //string strLogName = Constants.logName;
            //// create error message
            //string strMessage = "Url " + this.Source + " causes error: " + this.Message;

            //// create log in case log is non existing
            //if (!EventLog.SourceExists(strLogName))
            //    EventLog.CreateEventSource(strLogName, strLogName);

            //EventLog evtLog = new EventLog();
            //evtLog.Source = strLogName;

            //// write error message to log
            //evtLog.WriteEntry(strMessage, EventLogEntryType.Error);
        }

        #endregion
    }
}
