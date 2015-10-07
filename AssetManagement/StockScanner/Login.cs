namespace StockScanner
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Security.Principal;
    using System.Web.Security;
    using System.Globalization;

    public partial class Login : Form
    {
        public Login()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.UICulture);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text;
            string password = textBox2.Text;
            if (Membership.ValidateUser(user, password))
            {
                IPrincipal principal = new GenericPrincipal(new GenericIdentity(user), new string[] {});
                System.Threading.Thread.CurrentPrincipal = principal;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.label3.Text = "Login or password is invalid!";
                this.label3.ForeColor = System.Drawing.Color.Red;
                this.textBox2.Focus();
                this.textBox2.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1_Click(this, e);
            }
        }
    }
}
