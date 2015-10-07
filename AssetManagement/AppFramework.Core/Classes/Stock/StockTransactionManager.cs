/*--------------------------------------------------------
* StockTransactionManager.cs
* 
* Author: Alexey Nesterov
* Created: 27.07.2009 11:04 AM
* Purpose: Сlasses for managing stock transactions
* 
* Revisions: Moved to Core (from StockScanner) - 27.07.2009 - Alexey Nesterov
* -------------------------------------------------------*/

namespace AppFramework.Core.Classes.Stock
{
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.Classes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Transaction status for log row
    /// </summary>
    public enum LogRecordStatus
    {
        /// <summary>
        /// Transaction created successfully
        /// </summary>
        OK,

        /// <summary>
        /// Transaction created, but with warnings
        /// </summary>
        Warning,

        /// <summary>
        /// Transaction declined
        /// </summary>
        Error
    }

    /// <summary>
    /// Describe on log record
    /// </summary>
    public struct LogRecord
    {
        /// <summary>
        /// Title - just for user friendly interface
        /// </summary>
        public string Title;

        /// <summary>
        /// Count of units
        /// </summary>
        public decimal Count;

        /// <summary>
        /// Price of one unit
        /// </summary>
        public decimal Price;

        /// <summary>
        /// Direction of transaction
        /// </summary>
        public TransactionTypeCode Direction;

        /// <summary>
        /// Status or row
        /// </summary>
        public LogRecordStatus Status;

        /// <summary>
        /// Text for Info column of grid
        /// </summary>
        public string Info;
        
        /// <summary>
        /// Info detatailed message - appers when Info link clicked
        /// </summary>
        public string InfoMessage;

        /// <summary>
        /// Gets the default record.
        /// </summary>
        /// <returns></returns>
        public static LogRecord GetDefaultRecord()
        {
            return new LogRecord()
            {
                Title = "Log record",
                Status = LogRecordStatus.OK,
                Price = 0,
                Count = 0,
                Direction = TransactionTypeCode.In,
                Info = "",
                InfoMessage = ""
            };
        }
    }

    public class StockTransactionManager
    {
        public List<string> Output
        {
            get;
            set;
        }

        public StockTransactionManager()
        {
            this.Output = new List<string>();
        }

        /// <summary>
        /// Creates and commits the transaction.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="count">The count.</param>
        /// <param name="price">The price.</param>
        /// <param name="description">The description.</param>
        /// <param name="endUserId">The department id.</param>
        /// <param name="assetId">The asset id.</param>
        public LogRecord CreateTransaction(
            IAuthenticationService authenticationService, IAssetsService assetsService,
            Asset asset, TransactionTypeCode direction, decimal count, decimal price, string description, long endUserId, long? locationId, long? fromLocationId)
        {
            StockTransaction trans = null;
            long assetConfigId = asset.GetConfiguration().ID;
            LogRecord rec = new LogRecord()
            {
                Title = string.Format("{0} - {1} transaction", asset.Name, Enum.GetName(direction.GetType(), direction)),                
                Price = price,
                Count = count,
                Direction = direction
            };

            this.AddMessage("Request processing started");
            // check, is threr enough rights to access this stock item information
            if (authenticationService.GetPermission(asset).CompareWithMask(Permission.RWRW.GetCode()))
            {
                switch (direction)
                {
                    case TransactionTypeCode.In:
                        trans = new StockTransactionIn(authenticationService, assetsService, assetConfigId, asset.ID, count, price, description, locationId);
                        break;

                    case TransactionTypeCode.Out:
                        trans = new StockTransactionOut(authenticationService, assetsService, assetConfigId, asset.ID, count, price, description, endUserId, fromLocationId);
                        break;

                    case TransactionTypeCode.Move:
                        trans = new StockTransactionMove(authenticationService, assetsService, asset.ID, assetConfigId, asset.ID, count, price, description, locationId, fromLocationId);
                        break;
                }

                var res = trans.Validate();
                if (res.IsValid)
                {
                    trans.Commit();
                    rec.Status = LogRecordStatus.OK;
                }
                else
                {
                    foreach (var item in res.ResultLines.Where(l => !l.IsValid).Select(l => l.Message))
                    {
                        this.AddMessage(item);
                    }
                    rec.Status = LogRecordStatus.Error;                    
                    rec.InfoMessage = "Errors in stock operation." + Environment.NewLine + string.Join(Environment.NewLine, res.ResultLines.Where(l => !l.IsValid).Select(l => l.Message).ToArray());
                }
            }   // rights check block
            else
            {
                rec.Status = LogRecordStatus.Error;
                rec.InfoMessage = string.Format("You have no enough rights to access this asset ({0}, barcode {1}), check if you have permissions to read and write financial info for this assets", asset.Name, asset["Barcode"].Value);
            }

            this.AddMessage("Request processing finished");
            this.AddMessage("============================");

            return rec;
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        private void AddMessage(string message)
        {
            this.Output.Add(string.Format("{0} - {1}", DateTime.Now.ToString(), message));
        }

        public Dictionary<long, decimal> GetAvailableLocationsFor(long assetId, long configId)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var locations = unitOfWork.GetStocksByLocation(assetId, configId);
            return locations.Where(l => l.RestCount > 0).ToDictionary(li => li.Location, li => li.RestCount);
        }

        private decimal GetRest(long? locationId, IEnumerable<Entities.DynEntityTransaction> transactions)
        {
            var stockIn = transactions.Where(t => t.LocationId == locationId && (t.TransactionTypeUid == 1 /* In */ || t.TransactionTypeUid == 3 /* Move */)).Sum(t => t.StockCount);
            var stockOut = transactions.Where(t => t.FromLocationId == locationId && t.TransactionTypeUid == 2).Sum(t => t.StockCount);

            return stockIn - stockOut;
        }
    }
}