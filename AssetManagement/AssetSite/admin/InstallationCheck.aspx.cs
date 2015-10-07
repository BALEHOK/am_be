using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes.Installer;
using System.IO;

namespace AssetSite.admin
{
    public partial class InstallationCheck : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string config = Request.QueryString["Config"];
            IntegrityChecker checker = null;
            if (!string.IsNullOrEmpty(config))
            {
                if (IntegrityChecker.IsConfigExists(config))
                {
                    checker = new IntegrityChecker(config);                    
                }
            }
            else
            {
                if (IntegrityChecker.IsConfigExists(IntegrityChecker.DefaultConfigName))
                {
                    checker = new IntegrityChecker(IntegrityChecker.DefaultConfigName);
                }                
            }

            if (checker != null)
            {
                ValidationReport.DataSource = checker.Rules;
                ValidationReport.DataBind();            
            }
        }
    }
}
