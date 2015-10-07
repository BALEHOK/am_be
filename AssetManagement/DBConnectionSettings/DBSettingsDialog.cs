using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DatabaseInstaller
{
    public partial class DBSettingsDialog : Form
    {

        #region Public properties

        public bool IsConnectionEstablished { get; private set; }

        public string Host
        {
            get
            {
                return cmbSqlServers.Text.Trim();
            }
        }

        public string Database
        {
            get
            {
                return txtDatabase.Text.Trim();
            }
        }

        public string User
        {
            get
            {
                return txtUser.Text.Trim();
            }
        }

        public string Password
        {
            get
            {
                return txtPassword.Text.Trim();
            }
        }

        public bool IsIntegratedSecurity
        {
            get
            {
                return chkIntSec.Checked;
            }
        }

        public string ConnectionString
        {
            get
            {
                return
                string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security={4};Connect Timeout=30;",
                Host, Database, User, Password, IsIntegratedSecurity);
            }            
        }
        
        #endregion

        public DBSettingsDialog()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            this.Cursor = Cursors.WaitCursor;  
        
            try
            {  
                if (Routines.IsConnectionOk(ConnectionString))
                {
                    txtMessages.Visible = false;
                    this.IsConnectionEstablished = true;
                    this.Hide();
                }                
            }
            catch (Exception ex)
            {
                txtMessages.Visible = true;
                txtMessages.Text = ex.Message;
            }

            this.Cursor = Cursors.Default;
            btnSave.Enabled = true;
        }

        private void cmbSqlServers_DropDown(object sender, EventArgs e)
        {
            if (cmbSqlServers.Items.Count == 0)
            {
                this.Cursor = Cursors.WaitCursor;
                cmbSqlServers.DataSource = Routines.GetSqlServers(true);
                this.Cursor = Cursors.Default;
            }
        }
    }
}
