using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace AssetSite.admin
{
    public partial class UsersMonitor : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Users.DataSource = Membership.GetAllUsers();
                Users.DataBind();
            }
        }
    }
}
