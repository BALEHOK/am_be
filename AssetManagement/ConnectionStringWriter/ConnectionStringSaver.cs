using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using DatabaseInstaller;
using DBConnectionSettings;
using System.Windows.Forms;


namespace ConnectionStringWriter
{
    [RunInstaller(true)]
    public partial class ConnectionStringSaver : Installer
    {
        public ConnectionStringSaver()
        {
            InitializeComponent();             
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            using (DBSettingsDialog dlg = new DBSettingsDialog())
            {
                dlg.ShowDialog();

                /*
                if (dlg.IsConnectionEstablished)
                {
                                        
                }
                else
                {
                    throw new Exception("Cannot connect to database.");
                }*/

                stateSaver.Add("cnnString", dlg.ConnectionString);
            }
        }

        public override void Commit(IDictionary savedState)
        {
            if (savedState.Contains("cnnString"))
            {            
                CnnWriter.Write(Context.Parameters["targetdir"].TrimEnd(new char[] { '/' }) + "/cnn.config",                                
                                savedState["cnnString"].ToString());
            }

            base.Commit(savedState);           
        }
    }
}
