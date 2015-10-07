namespace AppFramework.Core.Classes.Stock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Store single report line
    /// </summary>
    public struct StockSubtotalLine
    {
        public decimal Count
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Builds report for rest items in stock system grouped by price for given date
    /// </summary>
    public class StockSubtotalReport
    {
        public DateTime ReportDate
        {
            get;
            set;
        }

        public long AssetTypeId
        {
            get;
            set;
        }

        public long AssetId
        {
            get;
            set;
        }

        public StockSubtotalReport(long assetTypeId, long assetId)
            : this(assetTypeId, assetId, DateTime.Now)
        {
        }

        public StockSubtotalReport(long assetTypeId, long assetId, DateTime reportDate)
        {
            this.ReportDate = reportDate;
            this.AssetTypeId = assetTypeId;
            this.AssetId = assetId;
        }

        public IEnumerable<StockSubtotalLine> GetAll()
        {
            foreach (var item in StockTransaction.GetAll(this.AssetTypeId, this.AssetId).GroupBy(t => t.StockPrice))
            {
                decimal price = item.Key;
                decimal count = 0;

                count = item.Where(t => t.TransactionType == TransactionTypeCode.In && !t.CloseDate.HasValue).Sum(t => t.RestCount);
                //    - item.Where(t => t.TransactionType != TransactionTypeCode.In).Sum(t => t.RestCount);

                if (count != 0)
                {
                    StockSubtotalLine line = new StockSubtotalLine()
                    {
                        Count = count,
                        Price = price
                    };

                    yield return line;
                }
            }
        }
    }
}
