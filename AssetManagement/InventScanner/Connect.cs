using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogTable.Grid;
using System.Data.SqlClient;
using System.Data.Common;
using System.Threading;

namespace InventScanner
{
    public partial class Connect : Form
    {
        private bool alreadyTried = false;

        public Connect()
        {
            InitializeComponent();            
        }

        public bool Connected
        {
            private set;
            get;
        }

        private void TryConnect()
        {
            bool result = true;            
            try
            {
                var unitOfWork = new AppFramework.DataProxy.UnitOfWork();
                unitOfWork.SqlProvider.ExecuteScalar("SELECT @@IDENTITY");
            }
            catch (Exception ex)
            {
                // dirty trick instead of multiple catch blocks with same action
                if (ex is SqlException || ex is DbException)
                {
                    result = false;                    
                }
                else
                {
                    throw;
                }
            }

            this.Connected = result;
        }

        private void StartConnect()
        {
            this.pictureBox1.Image = ResourceLoader.GetResource("arrow-move") as Image;
            this.label2.Text = "Connecting...";
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.button1.Enabled = false;
        }

        private void BadConnect()
        {
            this.button1.Enabled = true;
            this.pictureBox1.Image = ResourceLoader.GetResource("cross") as Image;
            this.label2.Text = "Connection failed";
            this.label2.ForeColor = System.Drawing.Color.Red;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ConnectToDatabase();
        }

        private void Connect_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Connected = false;
            this.Close();
        }

        private void Connect_Activated(object sender, EventArgs e)
        {
            if (!alreadyTried)
            {
                alreadyTried = true;
                ConnectToDatabase();               
            }            
        }

        private void ConnectToDatabase()
        {
            this.StartConnect();
            Action a = new Action(this.TryConnect);
            a.BeginInvoke(new AsyncCallback(ConnectCallback), this);
        }

        private void ConnectCallback(IAsyncResult result)
        {            
            Connect connect = result.AsyncState as Connect;
            if (connect != null)
            {
                if (!connect.Connected)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.BadConnect();
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                    });
                }
            }
        }
    }
}