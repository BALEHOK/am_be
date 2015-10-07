using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Resources;
using DatabaseInstaller.Properties;
using System.Data.Sql;

namespace DatabaseInstaller
{
    internal static class Routines
    {
        /// <summary>
        /// Performs the SQL command execution within transaction.
        /// </summary>
        /// <param name="sqlCommandText">SQL command</param>
        /// <param name="cnnString">Connection string</pparam>
        public static void ExecuteBatchWithTransaction(string cnnString, string sqlCommandText)
        {
            if (cnnString == string.Empty || sqlCommandText == string.Empty)
            {
                throw new ArgumentException(string.Format("Cannot perform SQL command while argument is missing."));
            }            

            using (SqlConnection cnn = new SqlConnection(cnnString))
            {
                cnn.Open();
                using (SqlTransaction trans = cnn.BeginTransaction())
                { 
                    // execute each statement separately since GO does not supports by .NET Sql Provider
                    Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string[] queries = regex.Split(sqlCommandText);

                    foreach (string query in queries.Where(q => q.Length > 0))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, cnn, trans))
                        {                            
                            cmd.ExecuteNonQuery();
                        }                        
                    }
                    trans.Commit();
                }
            }            
        }

        /// <summary>
        /// Performs the SQL command execution.
        /// </summary>
        /// <param name="sqlCommandText">SQL command</param>
        /// <param name="cnnString">Connection string</pparam>
        public static void ExecuteSQL(string cnnString, string sqlCommandText)
        {
            using (SqlConnection cnn = new SqlConnection(cnnString))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(sqlCommandText, cnn))
                {       
                    cmd.ExecuteNonQuery();
                }            
            }
        } 
    }
}
