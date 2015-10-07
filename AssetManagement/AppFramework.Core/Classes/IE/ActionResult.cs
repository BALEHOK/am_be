using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE
{
    /// <summary>
    /// Holds the results of any convertion operation or any another action 
    /// with status and object as output
    /// </summary>
    /// <typeparam name="T">Type of result object</typeparam>
    public class ActionResult<T>
    {
        public StatusInfo Status { get; set; }
        public T Data { get; set; }
        private ILog _logger = LogManager.GetCurrentClassLogger();
        public ActionResult() 
        {
            try
            {
                Data = Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            
            Status = new StatusInfo();
        }

        public ActionResult(StatusInfo status, T data) 
            : this()
        {
            Status = status;
            Data = data;
        }
    }
}
