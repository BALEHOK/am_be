using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace AssetSite.Controls
{
    public partial class NewsAndLoginInfo : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] newsList = {
                              "Lewis Hamilton", 
                              "Heikki Kovalainen",
                              "Felipe Massa",
                              "Kimi Raikkonen",
                              "Robert Kubica",
                              "Nick Heidfeld",
                              "Fernando Alonso",
                              "Nelson Piquet Jr",
                              "Jarno Trulli",
                              "Timo Glock",
                              "Sebastien Bourdais",
                              "Sebastien Buemi",
                              "Mark Webber",
                              "Sebastian Vettel",
                              "Nico Rosberg",
                              "Kazuki Nakajima",
                              "Adrian Sutil",
                              "Giancarlo Fisichella",
                              "Jenson Button",
                              "Rubens Barrichello"
                          };

            IEnumerable result = from news in newsList
                                 orderby news
                                 select new
                                 {
                                     Date = DateTime.Now.ToShortDateString(),
                                     Text = news,
                                     Title = news,
                                     Url = "http://wasm.ru"
                                 };
            NewsList.DataSource = result;
            NewsList.DataBind();
        }
    }
}