using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DevelopmentInformationService.Account
{
    public partial class Manage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["DB"] != null)
                {
                    XmlDataBase db = (XmlDataBase)Session["DB"];

                    tbVersion.Text = db.Version;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["DB"] != null)
            {
                XmlDataBase db = (XmlDataBase)Session["DB"];

                db.Version = tbVersion.Text;
            }
        }
    }
}