using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
using System.Windows;

namespace AssetUpdater.Code
{
    public class PackageProcessor
    {
        public event PackegProcessorProgressDelegate OnProgress, OnActionEnd, OnActionBegin;

        public bool IsSQL { get; set; }
        public string PackagePath { get; set; }
        public string WebsitePath { get; set; }
        public string Version { get; set; }

        public PackageProcessor() { }

        public PackageProcessor(bool isSql, string pak_path)
        {
            this.IsSQL = isSql;
            this.PackagePath = pak_path;
        }

        public void ExecutePackage()
        {
            if (this.IsSQL)
            {
                if (OnActionBegin != null)
                    OnActionBegin(new object(), new Arguments.PackageProcessorProgressEventArgs() { Info = "Unzipping package...", Progress = -1, ActionID = "unzip" });

                string packPath = this.UnzipPackage();

                if (OnActionEnd != null)
                    OnActionEnd(new object(), new Arguments.PackageProcessorProgressEventArgs() { Info = "Unzipped", ActionID = "unzip" });

                if (OnActionBegin != null)
                    OnActionBegin(new object(), new Arguments.PackageProcessorProgressEventArgs() 
                    { Info = "Database Update...", ActionID="updatedb" });

                this.ProcessSQL(packPath);

                if (OnActionEnd != null)
                    OnActionEnd(new object(), new Arguments.PackageProcessorProgressEventArgs() { Info = "Updated", ActionID = "updatedb" });

                Directory.Delete(packPath, true);
            }
            else
            {
                if (OnActionBegin != null)
                    OnActionBegin(new object(), new Arguments.PackageProcessorProgressEventArgs() { Info = "Website Update...", ActionID = "updateweb", Progress = -1 });

                this.UpdateWebsite();

                if (OnActionEnd != null)
                    OnActionEnd(new object(), new Arguments.PackageProcessorProgressEventArgs() { Info = "Updated", ActionID="updateweb" });
            }


        }

        private string UnzipPackage()
        {
            string directoryName = Path.Combine(Directory.GetCurrentDirectory(), "Package" + DateTime.Now.Ticks);

            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(this.PackagePath, directoryName, FastZip.Overwrite.Always, null, null, null, false);

            return directoryName;
        }

        private void ProcessSQL(string unzippedPak)
        {
            string connectionString = string.Empty;

            string connStringFilePath = this.WebsitePath.TrimEnd(new char[1] { '\\' }) + @"\Config.Files\connectionStrings.config";
            if (File.Exists(connStringFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(connStringFilePath);

                XmlNode node = doc.SelectSingleNode("//add[@key='SetupVersion']");
                if (node != null)
                {
                    connectionString = node.Attributes["connectionString"].Value;
                    MessageBox.Show(connectionString);
                }
            }
            else
                return;

            //InstallDatabase(connectionString, unzippedPak);
            //UpdateDatabaseVersion(connectionString);
            //UpdateApplicationVersion();
        }

        private void UpdateWebsite()
        {
            FastZip unzipper = new FastZip();
            unzipper.ExtractZip(this.PackagePath, this.WebsitePath, FastZip.Overwrite.Always, null, null, null, false);
        }

        #region SQL Execution
        private void InstallDatabase(string cnnString, string unzippedPath)
        {
            DirectoryInfo di = new DirectoryInfo(unzippedPath);
            List<FileInfo> files = di.GetFiles().ToList();

            long step = Convert.ToInt64(Math.Round((double)files.Count / 100));
            long progress = 0;

            foreach (FileInfo fi in files)
            {
                if (fi.Extension.ToLower().Contains("sql"))
                {
                    using (StreamReader fs = fi.OpenText())
                    {
                        Routines.ExecuteBatchWithTransaction(cnnString, fs.ReadToEnd());
                        progress += step;

                        if (OnProgress != null)
                            OnProgress(new object(), new Arguments.PackageProcessorProgressEventArgs() { Progress = progress });
                    }
                }
            }
        }

        private void UpdateDatabaseVersion(string cnnString)
        {
            Routines.ExecuteSQL(cnnString,
                string.Format("UPDATE [AppSettings] SET [PropertyValue] = '{0}' WHERE [PropertyName] = 'DatabaseVersion';",
                this.Version));
        }

        /// <summary>
        /// Writes current installation version into application config
        /// </summary>
        /// <param name="configPath">Full path to config file.</param>
        /// <param name="version">Installation version.</param>
        private void UpdateApplicationVersion()
        {

            string configPath = this.WebsitePath + @"\Config.Files\appSettings.config";

            XmlDocument XMLDoc = new XmlDocument();

            try
            {
                XMLDoc.Load(configPath);
                XmlNode node = XMLDoc.SelectSingleNode("//add[@key='SetupVersion']");

                node.Attributes["value"].Value = this.Version;

                if (node != null)
                {
                    if (node.Attributes["value"] != null)
                    {
                        node.Attributes["value"].Value = this.Version;
                    }
                }

                XMLDoc.Save(configPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Unable to set application version: {0}", ex.Message));
            }
        }
        #endregion
    }
}
