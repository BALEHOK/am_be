//-----------------------------------------------------------------------
// <copyright file="LogTable.cs" company="">
//     Copyright (c) .  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace LogTable
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Resources;
    using System.Text;
    using System.Windows.Forms;
    using AppFramework.Core.Classes.Stock;    
    using System.IO;

    /// <summary>
    /// Log table for display transaction execution status
    /// </summary>
    public class LogTable : DataTable
    {
        private List<LogRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTable"/> class.
        /// </summary>
        public LogTable()
        {
            this.TableName = "LogTable";
            this.Columns.Add(new DataColumn("Title", typeof(string)));
            this.Columns.Add(new DataColumn("Direction", typeof(TransactionTypeCode)));
            this.Columns.Add(new DataColumn("Count", typeof(float)));
            this.Columns.Add(new DataColumn("Price", typeof(float)));
            this.Columns.Add(new DataColumn("Status", typeof(LogRecordStatus)));
            this.Columns.Add(new DataColumn("Info", typeof(string)));
            this.Columns.Add(new DataColumn("InfoMessage", typeof(string)));            
        }

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="status">The status.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="count">The count.</param>
        /// <param name="price">The price.</param>
        public void AddRow(string title, LogRecordStatus status, TransactionTypeCode direction, decimal count, decimal price, string info)
        {
            this.AddRow(title, status, direction, count, price, info, 0, 0);
        }

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="status">The status.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="count">The count.</param>
        /// <param name="price">The price.</param>
        /// <param name="departmentId">The department id.</param>
        /// <param name="assetUid">The asset uid.</param>
        public void AddRow(string title, LogRecordStatus status, TransactionTypeCode direction, decimal count, decimal price, string info, long departmentId, long assetUid)
        {
            this.Rows.Add(title, direction, count, price, status, string.IsNullOrEmpty(info) ? "" : "View message...", info);
        }

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="rec">The rec.</param>
        public void AddRow(LogRecord rec)
        {
            this.AddRow(rec.Title, rec.Status, rec.Direction, rec.Count, rec.Price, rec.InfoMessage);
        }

        /// <summary>
        /// Saves the log.
        /// </summary>
        /// <param name="logPath">The log path.</param>
        public void SaveLog(string logPath)
        {
            try
            {
                this.WriteXml(logPath);
            }
            catch
            {
                MessageBox.Show("Saving last session log failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }


        /// <summary>
        /// Loads the log.
        /// </summary>
        /// <param name="p">The path.</param>        
        public void LoadLog(string path)
        {
            try
            {
                this.ReadXml(path);
            }
            catch
            {
                MessageBox.Show("Loading last session log failed. Session restarted", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
    }
}
