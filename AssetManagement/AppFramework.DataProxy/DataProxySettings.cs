using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace AppFramework.DataProxy
{
    /// <summary>
    /// Contains global settings for data proxy classes
    /// </summary>
    public static class DataProxySettings
    {
        #region private members

        private static int commandTimeout = 30;//default value for Command timeout

        #endregion

        #region properties

        /// <summary>
        /// Cached value for Command timeout
        /// </summary>
        public static int CommandTimeout
        {
            get { return commandTimeout; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static DataProxySettings()
        {
            string commandTimeoutString = ConfigurationManager.AppSettings["CommandTimeout"];
            if (commandTimeoutString != null)
            {
                int.TryParse(ConfigurationManager.AppSettings["CommandTimeout"], out commandTimeout);
            }
        }

        #endregion
    }
}
