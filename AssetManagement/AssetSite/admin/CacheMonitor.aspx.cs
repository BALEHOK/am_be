using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AppFramework.Core.Classes.Caching;
using AppFramework.Core.Classes;

namespace AssetSite.admin
{
    public partial class CacheMonitor : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ObjCount.Text = AppFramework.Core.Classes.Caching.Cache<CacheMonitor>.ObjCount.ToString();
            MemUsage.Text = AppFramework.Core.Classes.Caching.Cache<CacheMonitor>.MemUsage.ToString();
        }

        protected void FlushCacheClicked(object sender, EventArgs e)
        {
            AppFramework.Core.Classes.Caching.Cache<CacheMonitor>.Flush();
            Response.Redirect(Request.Url.OriginalString);
        }
    }
}
