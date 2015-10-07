using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AssetSite
{
    public class HttpContextVersionProvider
    {
        public override string ToString()
        {
            return HtmlHelperExtensions.GetAppBuildVersion();
        }
    }
}
