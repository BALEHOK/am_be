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
using System.Data.Sql;

namespace DatabaseInstaller
{
    internal static class Routines
    {
        /// <summary>
        /// Checks the connection to DB
        /// </summary>
        /// <param name="cnnString">Connection string</param>
        /// <returns>True if connected</returns>
        public static bool IsConnectionOk(string cnnString)
        {
            bool isEstablished = false;

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = cnnString;

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
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return isEstablished;
        }

        /// <summary>
        /// A method that returns a list of all available SQL Servers in the network
        /// </summary>
        /// <param name="shouldSortList">Specifies whether the list should be sorted or not</param>
        /// <returns>a generic list of string containing the servers found</returns>
        public static List<string> GetSqlServers(bool shouldSortList)
        {
            //create the list that would hold our servers
            List<string> listOfServers = new List<string>();

            //create a new instance of our SqlDataSourceEnumerator
            SqlDataSourceEnumerator sqlEnumerator = SqlDataSourceEnumerator.Instance;

            //get the datatable containing our sql servers
            DataTable sqlServersTable = sqlEnumerator.GetDataSources();

            //iterate thru all the rows
            foreach (DataRow rowOfData in sqlServersTable.Rows)
            {
                //get the server name
                string serverName = rowOfData["ServerName"].ToString();
                //get the instance name
                string instanceName = rowOfData["InstanceName"].ToString();

                //check if the instance name is empty
                if (!instanceName.Equals(String.Empty))
                {
                    //append the instance name to the server name
                    serverName += String.Format("\\{0}", instanceName);
                }

                //add the server to our list
                listOfServers.Add(serverName);
            }

            //sort the list if the sort option is specified
            if (shouldSortList)
            {
                //sort it!
                listOfServers.Sort();
            }

            //return our list
            return listOfServers;
        }
    }
}
