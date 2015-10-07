using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetSite
{
    public class LoggerConfig
    {
        public static void Configure()
        {
            log4net.GlobalContext.Properties["user"]
                = new HttpContextUserNameProvider();
            log4net.GlobalContext.Properties["version"]
                = new HttpContextVersionProvider();
        }
    }
}