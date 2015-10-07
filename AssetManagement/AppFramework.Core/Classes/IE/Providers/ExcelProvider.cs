using AppFramework.ConstantsEnumerators;
using Common.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE.Providers
{
    public class ExcelProvider
    {
        public StatusInfo Status { get; set; }
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public ExcelProvider()
        {
            Status = new StatusInfo();
        }

        private OleDbConnection GetConnection(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
                throw new ArgumentNullException("FileName");

            string cnnProvider;
            string version;

            if (filepath.EndsWith("xlsx"))
            {
                cnnProvider = @"Provider=Microsoft.ACE.OLEDB.12.0;";
                version = "12.0";
            }
            else if (filepath.EndsWith("xls"))
            {
                cnnProvider = @"Provider=Microsoft.Jet.OLEDB.4.0;";
                version = "8.0";
            }
            else
            {
                throw new ArgumentException("Unknown file format");
            }

            var sb = new StringBuilder();
            sb.Append(cnnProvider);
            sb.AppendFormat("Data Source={0};Mode=ReadWrite;", filepath);
            sb.AppendFormat("Extended Properties=\"Excel {0}; MaxScanRows=0; IMEX=0\"", version);
            return new OleDbConnection(sb.ToString());
        }

        /// <summary>
        /// Returhs the Collection of KeyValue pairs 
        /// where key - sheet of datasource
        /// value - field in sheet
        /// </summary>
        /// <param name="sheets"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetFields(string filepath, List<string> sheets)
        {
            foreach (var sheet in sheets)
            {
                var table = GetDataTable(filepath, sheet);
                foreach (DataColumn column in table.Columns)
                {
                    yield return new KeyValuePair<string, string>(sheet, column.ColumnName);
                }
            }
        }

        /// <summary>
        /// Returns the DataSet based on file source
        /// </summary>
        /// <returns></returns>
        public ActionResult<DataSet> GetDataSet(string filepath, IEnumerable<string> sheets)
        {
            var ds = new DataSet();
            foreach (string sheet in sheets)
            {
                ds.Tables.Add(GetDataTable(filepath, sheet));
            }
            return new ActionResult<DataSet>(Status, ds);
        }

        /// <summary>
        /// Returns the datatable based on sheet with given name
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="strSheetName"></param>
        /// <returns></returns>
        private DataTable GetDataTable(string filepath, string strSheetName)
        {
            DataTable dt = null;
            using (var connection = GetConnection(filepath))
            {
                try
                {
                    var strComand = "select * from [" + strSheetName + "$]";
                    var daAdapter = new OleDbDataAdapter(strComand, connection);
                    dt = new DataTable(strSheetName);
                    daAdapter.FillSchema(dt, SchemaType.Source);
                    daAdapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    Status.IsSuccess = false;
                    Status.Errors.Add(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return dt;
        }

        /// <summary>
        /// dINSERTS a table to a Xls
        /// </summary>
        /// <param name="dt">DataTable to insert</param>
        /// <returns></returns>
        public void Write(DataTable dt, string filepath)
        {
            using (var cnn = GetConnection(filepath))
            {
                try
                {
                    var crTableCmdBuilder = new StringBuilder();
                    var insertRowCmdBuilder = new StringBuilder();
                    var insertRowValuesCmdBuilder = new StringBuilder();
                    var insertCommand = new OleDbCommand();

                    string tableName = string.IsNullOrEmpty(dt.TableName) ? "Sheet1$" : dt.TableName;
                    crTableCmdBuilder.AppendFormat("CREATE TABLE [{0}] (", tableName);
                    insertRowCmdBuilder.AppendFormat("INSERT INTO [{0}] (", tableName);

                    foreach (DataColumn dc in dt.Columns)
                    {
                        crTableCmdBuilder.AppendFormat("[{0}] char(255),", dc.ColumnName);
                        insertRowCmdBuilder.AppendFormat("[{0}],", dc.ColumnName);
                        insertRowValuesCmdBuilder.AppendFormat("[@{0}],", dc.ColumnName);
                        insertCommand.Parameters.Add(String.Format("[@{0}]", dc.ColumnName), OleDbType.VarChar);

                    }
                    crTableCmdBuilder.Replace(",", "", crTableCmdBuilder.Length - 1, 1);
                    crTableCmdBuilder.Append(")");
                    insertRowCmdBuilder.Replace(",", "", insertRowCmdBuilder.Length - 1, 1);
                    insertRowCmdBuilder.Append(") ");
                    insertRowValuesCmdBuilder.Replace(",", "", insertRowValuesCmdBuilder.Length - 1, 1);
                    insertRowCmdBuilder.AppendFormat("VALUES ({0})", insertRowValuesCmdBuilder);

                    //create sheet
                    var cmd = new OleDbCommand(crTableCmdBuilder.ToString(), cnn);
                    cnn.Open();
                    cmd.ExecuteNonQuery();

                    //Fill sheet with data
                    var daAdapter = new OleDbDataAdapter("select * from [" + tableName + "]", cnn);
                    insertCommand.CommandText = insertRowCmdBuilder.ToString();
                    insertCommand.Connection = cnn;
                    daAdapter.InsertCommand = insertCommand;
                    daAdapter.Update(dt);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    Status.IsSuccess = false;
                    Status.Errors.Add(ex.Message);
                }
                finally
                {
                    cnn.Close();
                }
            }
        }

        /// <summary>
        /// INSERTS a table with data to a Xls usin EPPlus
        /// </summary>
        /// <param name="dt">DataTable to insert</param>
        /// <returns></returns>
        public void WriteWithData(DataTable dt, string fileName)
        {
            FileInfo newFile = new FileInfo(fileName);
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                if (pck.Workbook.Worksheets.Count == 0)
                {
                    pck.Workbook.Worksheets.Add("Assets");
                }

                ExcelWorksheet ws = pck.Workbook.Worksheets.First();
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                pck.Save();
                //try to release stream
                try
                {
                    pck.Stream.Flush();
                    pck.Stream.Close();
                }
                catch
                {
                    //empty catch
                }
            }
        }

        /// <summary>
        /// Returns the string array of all sheet names
        /// </summary>
        /// <returns></returns>
        public List<string> GetExcelSheetNames(string filepath)
        {
            var excelSheets = new List<string>();
            using (var cnn = GetConnection(filepath))
            {
                try
                {
                    cnn.Open();
                    // Get the data table containing the schema
                    DataTable dt = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                        return null;

                    int i = 0;
                    // Add the sheet name to the string array.
                    foreach (DataRow row in dt.Rows)
                    {
                        string strSheetTableName = row["TABLE_NAME"].ToString();
                        excelSheets.Add(strSheetTableName.Substring(0, strSheetTableName.Length - 1));
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    Status.IsSuccess = false;
                    Status.Errors.Add(ex.Message);
                }
                finally
                {
                    cnn.Close();
                }
            }
            return excelSheets;
        }
    }
}
