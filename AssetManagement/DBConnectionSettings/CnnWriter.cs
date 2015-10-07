using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DBConnectionSettings
{
    public static class CnnWriter
    {
        /// <summary>
        /// Saves the connection string to .config file
        /// </summary>
        /// <param name="filePath">Full path to file</param>
        /// <param name="cnnString">Connection string</param>
        public static void Write(string filePath, string cnnString)
        {
            TextWriter wr = new StreamWriter(filePath);

            string settings =
                string.Format("<connectionStrings>{1}<clear />{1}"
                            + "<add name=\"netTiersConnectionString\" connectionString=\"{0}\" />{1}"                     
                            + "</connectionStrings>",
                            cnnString, Environment.NewLine);
            try
            {
                wr.Write(settings);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                wr.Close();
            }
        }
    }
}
