using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;
using DatabaseInstaller.Properties;
using System.Windows.Forms;
using AppFramework.Core.Classes;
using DBConnectionSettings;
using System.Xml;
using System.Globalization;

namespace DatabaseInstaller
{
    [RunInstaller(true)]
    public partial class DBInstaller : Installer
    { 
        public DBInstaller()
        {
            InitializeComponent();          
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            using (DBSettingsDialog dlg = new DBSettingsDialog())
            {
                dlg.ShowDialog();

                if (dlg.IsConnectionEstablished)
                {
                    stateSaver.Add("cnnString", dlg.ConnectionString);
                    stateSaver.Add("DATABASE", dlg.Database);
                }
                else
                {
                    throw new Exception("Cannot perform the database installation.");
                }
            }

            if (stateSaver.Contains("cnnString"))
            {   
                InstallDatabase(stateSaver["cnnString"].ToString());                
            }
            else
            {
                throw new Exception("Connection string was not provided.");
            }                
        }

        public override void Commit(IDictionary savedState)
        {
            if (!Context.Parameters.ContainsKey("ProductVersion"))
            {
                throw new ArgumentException("ProductVersion property was not provided.");
            }

            if (!Context.Parameters.ContainsKey("targetdir"))
            {
                throw new ArgumentException("targetdir property was not provided.");
            }

            if (!savedState.Contains("cnnString"))
            {        
                throw new ArgumentException("Connection string was not provided.");
            }

            SaveConnectionStringToAppConfig(savedState["cnnString"].ToString());
            UpdateDatabaseVersion(savedState["cnnString"].ToString());
            UpdateApplicationVersion();

            base.Commit(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            if (!savedState.Contains("cnnString"))
            {
                throw new ArgumentException("Connection string was not provided.");
            }

            if (MessageBox.Show(string.Format("Drop application database '{0}'?", savedState["DATABASE"].ToString()), 
                "DROP DATABASE", MessageBoxButtons.YesNo) 
                == DialogResult.Yes)
            {
                DropDatabase(savedState["cnnString"].ToString(), savedState["DATABASE"].ToString());
            }

            base.Uninstall(savedState);
        }

        private void InstallDatabase(string cnnString)
        {
            Routines.ExecuteBatchWithTransaction(cnnString, Resources.database);
            Routines.ExecuteSQL(cnnString, "EXEC [dbo].[_DropAssetTables]");
        }

        private void DropDatabase(string cnnString, string database)
        {
            Routines.ExecuteSQL(cnnString, Resources.dropdatabase.Replace("%%DATABASE%%", database));            
        }

        private void SaveConnectionStringToAppConfig(string cnnString)
        {
            if (Context.Parameters.ContainsKey("targetdir"))
            {
                string filePath = Context.Parameters["targetdir"].TrimEnd(new char[] { '\\' })
                                    + @"\"
                                    + Resources.coonectitonStringsRelPath.TrimStart(new char[] { '\\' });
                CnnWriter.Write(filePath, cnnString);
            }
            else
            {
                throw new ArgumentException("targetdir property was not provided.");
            }
        }

        private void UpdateDatabaseVersion(string cnnString)
        {
            Routines.ExecuteSQL(cnnString, 
                string.Format("UPDATE [AppSettings] SET [PropertyValue] = '{0}' WHERE [PropertyName] = 'DatabaseVersion';",
                Context.Parameters["ProductVersion"].ToString()));
        }

        /// <summary>
        /// Writes current installation version into application config
        /// </summary>
        /// <param name="configPath">Full path to config file.</param>
        /// <param name="version">Installation version.</param>
        private void UpdateApplicationVersion()
        {

            string configPath = Context.Parameters["targetdir"].TrimEnd(new char[] { '\\' })
                                + @"\"
                                + Resources.appSettingsRelPath.TrimStart(new char[] { '\\' });
            string version = Context.Parameters["ProductVersion"].ToString();

            XmlDocument XMLDoc = new XmlDocument();
            
            try
            {
                XMLDoc.Load(configPath);
                XmlNode node = XMLDoc.SelectSingleNode("//add[@key='SetupVersion']");

                node.Attributes["value"].Value = version;

                if (node != null)
                {
                    if (node.Attributes["value"] != null)
                    {
                        node.Attributes["value"].Value = version;
                    }
                }

                XMLDoc.Save(configPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Unable to set application version: {0}", ex.Message));
            }            
        }
    }
}
