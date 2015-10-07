using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.Classes
{
    public static class Routines
    {
        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random(DateTime.Now.Millisecond);
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        /// <summary>
        /// Generates a random uppercase string
        /// </summary>
        /// <returns></returns>
        public static string RandomString()
        {
            Random random = new Random();
            return Routines.RandomString(random.Next(3, 8), false);
        }

        /// <summary>
        /// Returns the enum item by its name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        /// <summary>
        /// Cleanes up the incorrect characters which cannot be used as table name or field name
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static string SanitizeDBObjectName(string inputName)
        {
            string outputString = inputName;

            // it's table name editing - remove the prefix
            if (outputString.StartsWith(Constants.DynTablePrefix))
                outputString = outputString.Replace(Constants.DynTablePrefix, string.Empty);

            Regex spaceRe = new Regex(@"\s+");
            outputString = spaceRe.Replace(outputString, "_");

            Regex re = new Regex(@"[^A-Za-z0-9_]");
            // clean up string
            outputString = re.Replace(outputString, string.Empty);

            // remove reserved words
            foreach (var word in getReserverWords())
            {
                if (word.ToLower() == outputString.ToLower())
                {
                    outputString = outputString + Routines.RandomString(1, false);
                }
            }

            // cut too long strings 
            if (outputString.Length > 20)
            {
                outputString = string.Format("{0}{1}",
                    outputString.Substring(0, 18),
                    Routines.RandomString(4, false));
            }
            else
            {
                while (outputString.Length < 4)
                    outputString += Routines.RandomString(1, false);
            }
            return outputString;
        }

        private static string[] getReserverWords()
        {
            if (HttpContext.Current != null)
            {
                var cache = HttpContext.Current.Cache;
                var words = cache["ReservedWords"] as List<string>;
                if (words == null)
                {
                    words = new List<string>();
                    string filepath = HttpContext.Current.Server.MapPath("~/Config.Files/reservedwords.txt");

                    if (File.Exists(filepath))
                    {
                        using (StreamReader sr = new StreamReader(filepath))
                        {
                            while (!sr.EndOfStream)
                            {
                                words.Add(sr.ReadLine().Trim());
                            }
                            sr.Close();
                        }

                        var cd = new System.Web.Caching.CacheDependency(filepath);
                        cache.Insert("ReservedWords", words, cd);
                    }
                    else
                    {
                        Trace.Write("Cannot find Config.Files/reservedwords.txt file");
                    }
                }
                return words.ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Cleanes up the incorrect characters from filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string SanitizeFileName(string filename)
        {
            Regex re = new Regex(@"[^A-Za-z0-9_\.]");
            // clean up string
            return re.Replace(filename, string.Empty);
        }

        public static string SanitizeLoginName(string login)
        {
            Regex re = new Regex(@"[^A-Za-z0-9_\.\s]");
            // clean up string
            return re.Replace(login, string.Empty);
        }

        /// <summary>
        /// Performs sanitizing of any paths to directories
        /// </summary>
        public static string SanitizeFolderPath(string path)
        {
            if (path.Substring(path.Length - 1, 1) != "\\")
            {
                path += "\\";
            }
            return path;
        }

        /// <summary>
        /// Converts any object to byte array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(Object obj)
        {
            byte[] res;
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, obj);
                res = fs.ToArray();
            }
            catch (SerializationException e)
            {
                return null;
            }
            finally
            {
                fs.Close();
            }
            return res;
        }

        /// <summary>
        /// Converts byte array to object
        /// </summary>
        /// <param name="Buffer"></param>
        /// <returns></returns>
        public static object ByteArrayToObject(Byte[] Buffer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(Buffer);
            try
            {
                return formatter.Deserialize(stream);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Builds the SQL connection string.
        /// </summary>
        /// <param name="host">SQL Server address or DNS name.</param>
        /// <param name="database">Database name.</param>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <param name="integSec">Integrated security</param>
        /// <returns></returns>
        public static string BuildConnectionString(string host, string database, string user, string password, bool integSec)
        {
            return
                string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security={4};Connect Timeout=30;",
                host, database, user, password, integSec);
        }

        /// <summary>
        /// Returns the specific section of connection string.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string GetSectionFromConnectionString(Enumerators.ConnectionString section)
        {
            string key = string.Empty;

            switch (section)
            {
                case Enumerators.ConnectionString.Host:
                    key = "Data Source";
                    break;

                case Enumerators.ConnectionString.Database:
                    key = "Initial Catalog";
                    break;

                case Enumerators.ConnectionString.User:
                    key = "User ID";
                    break;

                case Enumerators.ConnectionString.Password:
                    key = "Password";
                    break;

                case Enumerators.ConnectionString.IsIntergatedSecurity:
                    key = "Integrated Security";
                    break;
            }

            Regex re = new Regex(string.Format("{0}=([^;]+);", key));
            Match m = re.Match(ConfigurationManager.ConnectionStrings[0].ConnectionString);

            string returnValue = string.Empty;
            if (m.Success && m.Groups.Count > 1)
            {
                returnValue = m.Groups[1].Value;
            }
            return returnValue;
        }

        /// <summary>
        /// Returns the specific section of connection string.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static void SetSectionValueToConnectionString(Enumerators.ConnectionString section, string value)
        {
            string key = string.Empty;

            switch (section)
            {
                case Enumerators.ConnectionString.Host:
                    key = "Data Source";
                    break;

                case Enumerators.ConnectionString.Database:
                    key = "Initial Catalog";
                    break;

                case Enumerators.ConnectionString.User:
                    key = "User ID";
                    break;

                case Enumerators.ConnectionString.Password:
                    key = "Password";
                    break;

                case Enumerators.ConnectionString.IsIntergatedSecurity:
                    key = "Integrated Security";
                    break;
            }

            string patternStr = string.Format("{0}=([^;]+);", key);
            string input = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            value = string.Format(string.Format("{0}={1};", key, value));
            string output = Regex.Replace(input, patternStr, value);
            output = output.Replace("\"", "&quot;");

            TextWriter wr = new StreamWriter(ApplicationSettings.CnnStringsFilePath);

            string settings =
                    string.Format("<connectionStrings> \n <clear /> \n <add name=\"DataEntities\" connectionString=\"{0} \" providerName=\"System.Data.EntityClient\"/>"
                                    + "</connectionStrings>",
                                     output);
            try
            {
                wr.Write(settings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                wr.Close();
            }

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
