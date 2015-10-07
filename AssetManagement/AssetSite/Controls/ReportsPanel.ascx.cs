using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace AssetSite.Controls
{
    public partial class ReportsPanel : System.Web.UI.UserControl
    {
        public Dictionary<string, Dictionary<string, object>> Reports { get; private set; }

        public ReportsPanel()
        {
            Reports = new Dictionary<string, Dictionary<string, object>>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var links = new List<KeyValuePair<string,string>>();
            foreach (var report in Reports)
            {
                var link = new KeyValuePair<string, string>(
                    report.Key, 
                    string.Format("/Reports/Render.aspx?{0}",
                        string.Join("&", from pair in report.Value
                                         select string.Format("{0}={1}", pair.Key, pair.Value))));
                links.Add(link);
            }
            Repeater.DataSource = links;
            Repeater.DataBind();
        }
    }
}