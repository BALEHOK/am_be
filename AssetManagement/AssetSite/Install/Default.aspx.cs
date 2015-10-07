using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes.Installer;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using AppFramework.Core.Classes;
using System.Configuration;
using System.Text.RegularExpressions;
using AppFramework.ConstantsEnumerators;
using AssetSite.Helpers;
using AppFramework.Core.AC.Authentication;
using System.Drawing;
using System.Web.Security;

namespace AssetSite.Install
{
    public partial class Default1 : BasePage
    {
        /// <summary>
        /// Gets the connection string builded from user input
        /// </summary>
        private string _connectionString
        {
            get
            {
                return Routines.BuildConnectionString(
                txtHost.Text.Trim(), txtDatabase.Text.Trim(), txtUser.Text.Trim(),
                txtPassword.Text.Trim(), chkIntSec.Checked);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtUser.Enabled = txtPassword.Enabled =
                    !chkIntSec.Checked;

                txtHost.Text = Routines.GetSectionFromConnectionString(Enumerators.ConnectionString.Host);
                txtDatabase.Text = Routines.GetSectionFromConnectionString(Enumerators.ConnectionString.Database);
                txtUser.Text = Routines.GetSectionFromConnectionString(Enumerators.ConnectionString.User);
                txtPassword.Text = Routines.GetSectionFromConnectionString(Enumerators.ConnectionString.Password);
                chkIntSec.Checked = Routines.GetSectionFromConnectionString(Enumerators.ConnectionString.IsIntergatedSecurity).ToLower()
                    == true.ToString().ToLower();
            }
        }

        protected void Apply_Click(object sender, EventArgs e)
        {

            bool isSettingsValid = true;

            #region Database settings
            if (IsConnectionEstablished())
            {
                // save the connection string to app config
                //ApplicationSettings.ConnectionString = _connectionString;
                ApplicationSettings.UpdateConnectionString(txtHost.Text, txtDatabase.Text, txtUser.Text, txtPassword.Text, chkIntSec.Checked.ToString());
                lblCnnState.Visible = false;
            }
            else
            {
                isSettingsValid = false;
                lblCnnState.Visible = true;
                lblCnnState.Text = "Cannot connect to Database. Please check the settings and try again.";

                // cannot perform following actions without DB connection
                return;
            }
            #endregion

            #region Administrator settings

            if (txtAdminOldPassword.Text.Length > 0 && txtAdminNewPassword.Text.Length > 0)
            {
                AssetUser admin = UserService.FindByName("admin");
                if (admin != null)
                {
                    if (admin.ChangePassword(txtAdminOldPassword.Text.Trim(), txtAdminNewPassword.Text.Trim()))
                    {
                        lblPwdStatus.Visible = true;
                        lblPwdStatus.Text = "Password was changed.";
                        lblPwdStatus.ForeColor = Color.Green;
                    }
                    else
                    {
                        isSettingsValid = false;
                        lblPwdStatus.Visible = true;
                        lblPwdStatus.Text = "Password was not changed.";
                        lblPwdStatus.ForeColor = Color.Red;
                    }
                }
                else
                {
                    throw new Exception("Cannot retrieve Administrator user.");
                }
            }

            #endregion

            if (isSettingsValid)
            {
                ApplicationSettings.ShowConfigurationPage = false;
                Response.Redirect("~/admin/");
            }
        }

        private bool IsConnectionEstablished()
        {
            bool isEstablished = false;
            SqlConnection connection = new SqlConnection();

            connection.ConnectionString = _connectionString;

            try
            {
                connection.Open();
                if ((connection.State & ConnectionState.Open) > 0)
                {
                    isEstablished = true;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return isEstablished;
        }
    }
}
