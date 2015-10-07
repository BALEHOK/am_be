using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockScanner
{
    public partial class ViewInfo : Form
    {
        public string InfoMessage;

        public ViewInfo()
        {
            InitializeComponent();            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox1.WordWrap = this.checkBox1.Checked;
        }

        private void ViewInfo_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.InfoMessage;
        }
    }
}