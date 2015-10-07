/*--------------------------------------------------------
* XmlAdapter.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/5/2009 11:11:22 AM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Serialization;
    using AppFramework.Core.Classes;
    using AppFramework.Core.Classes.IE;
    using AppFramework.Core.Classes.Stock;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using AppFramework.ConstantsEnumerators;
    using System.Data.SqlClient;
    using AppFramework.Core.Classes.SearchEngine;

    public struct InventResult
    {
        public string Name;
        public LogRecordStatus Status;
        public long AssetId;
        public long AssetTypeId;
        public string Message;
        public string Barcode;
        public bool Duplicate;
        public string CurrentLocation;
        public long CurrentLocationId;
    }

    internal class RowBarcodeComparer : IEqualityComparer<DataRow>
    {
        #region IEqualityComparer<DataRow> Members

        public bool Equals(DataRow x, DataRow y)
        {
            bool equal = false;
            if (x["Barcode"] != null && y["Barcode"] != null)
            {
                equal = string.Equals(x["Barcode"], y["Barcode"]) && string.Equals(x["Name"], y["Name"]);
            }
            return equal;
        }

        public int GetHashCode(DataRow obj)
        {
            return obj["Barcode"].ToString().ToLower().GetHashCode(); // obj.GetHashCode();
        }

        #endregion
    }

    internal class XmlAdapterComparer : IEqualityComparer<XmlAdapter>
    {
        #region IEqualityComparer<DataRow> Members

        public bool Equals(XmlAdapter x, XmlAdapter y)
        {
            return x.LocationId == y.LocationId;
        }

        public int GetHashCode(XmlAdapter obj)
        {
            return obj.LocationId.GetHashCode(); // obj.GetHashCode();
        }

        #endregion
    }

    [Serializable]
    public class XmlAdapter
    {
        private KeyValuePairs loadedFiles;
        private AssetsSet assetsSet;
        private AssetTypeDictionary assetTypes;
        private ReportList reportList;
        private bool isFinished;
        private bool isSynchronized;

        public XmlAdapter()
        {
            this.loadedFiles = new KeyValuePairs();
            this.assetsSet = new AssetsSet();
            this.assetTypes = new AssetTypeDictionary();
            this.reportList = new ReportList(this);
        }

        [XmlIgnore]
        public AssetsTable AssetsTable
        {
            get { return this.assetsSet.AssetsTable; }
        }

        [XmlIgnore]
        public LogTable LogTable
        {
            get { return this.assetsSet.LogTable; }
        }

        [XmlElement]
        public AssetsSet AssetSet
        {
            get { return this.assetsSet; }
            set { this.assetsSet = value; }
        }

        [XmlElement]
        public KeyValuePairs LoadedFiles
        {
            get { return this.loadedFiles; }
            set { this.loadedFiles = value; }
        }

        [XmlElement]
        public AssetTypeDictionary AssetTypes
        {
            get { return this.assetTypes; }
            set { this.assetTypes = value; }
        }

        [XmlElement]
        public long LocationId
        {
            get;
            set;
        }

        [XmlElement]
        public ReportList ReportList
        {
            get { return this.reportList; }
            set { this.reportList = value; }
        }

        [XmlElement]
        public bool IsFinished
        {
            get
            {
                return this.isFinished;
            }
            set
            {
                this.isFinished = value;
                if (this.ScanStatusChanged != null) this.ScanStatusChanged.Invoke(value);
            }
        }

        [XmlElement]
        public DateTime FinishTime
        {
            get;
            set;
        }

        [XmlElement]
        public string LocationName
        {
            get;
            set;
        }

        [XmlIgnore]
        public long LocationOut
        {
            get;
            set;
        }

        [XmlElement]
        public bool IsSynchronized
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsPreviewed
        {
            get;
            set;
        }

        public delegate void ReportListChangedHandler();
        public event ReportListChangedHandler ReportListChanged;

        public delegate void ScanStatusChangedHandler(bool isFinished);
        public event ScanStatusChangedHandler ScanStatusChanged;

        public event EventHandler<LengthlyOperationStepEventArgs> AssetMoved;

        public event EventHandler<LengthlyOperationStartEventArgs> AssetMoveStart;
        public event EventHandler AssetMoveEnd;

        public event EventHandler<LengthlyOperationStartEventArgs> PreviewStart = delegate { };
        public event EventHandler<LengthlyOperationStepEventArgs> PreviewStep = delegate { };
        public event EventHandler PreviewEnd = delegate { };

        public event EventHandler<LengthlyOperationStartEventArgs> SyncStart = delegate { };
        public event EventHandler<LengthlyOperationStepEventArgs> SyncStep = delegate { };
        public event EventHandler SyncEnd = delegate { };


        /// <summary>
        /// Loads assets from database.
        /// </summary>
        /// <param name="locationId">The location id for which assets will be loaded.</param>
        /// <param name="path">The path where loaded assets will be stored as XML files.</param>
        public void LoadFromDatabase(Asset location, string path)
        {
            var assets = AssetFactory.GetAssetsFromLocation(location.ID);
            foreach (var item in assets.GroupBy(a => a.GetConfiguration().ID))
            {
                // import all assets for asset type into file and add to list loaded files as pair - filename, asset type uid
                this.loadedFiles.AddRange(
                    ImportExportManager.ExportAssets(item, path).Data.Select(p => new KeyValue<long, string>(p.Key, p.Value)).ToList());

                // now all imported asset types stored in loadedFiles pair as keys. Import them into dictionary
                foreach (var lFile in this.loadedFiles)
                {
                    if (!this.assetTypes.ContainsKey(lFile.Key))
                    {
                        this.assetTypes.Add(lFile.Key, AssetType.GetByID(lFile.Key));
                    }
                }
            }

            foreach (var asset in assets)
            {
                if (asset != null)
                {
                    this.assetsSet.AssetsTable.Rows.Add(
                        asset.Name,
                        asset["Barcode"].Value,
                        asset.GetConfiguration().Name,
                        asset["OwnerId"].Value,
                        asset["DynEntityId"].Value,
                        asset.GetConfiguration().ID
                    );
                }
            }

            if (location != null)
            {
                this.LocationName = location.Name;
                this.LocationId = long.Parse(location["DynEntityId"].Value);
            }
        }

        internal AssetEx GetAsset(long assetUid, long assetTypeUid)
        {
            AssetEx asset = new AssetEx();
            string fileName = this.loadedFiles.FindByKey(assetTypeUid).Value;
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
                    xnm.AddNamespace("df", @"http://tempuri.org/AssetManagementAssets.xsd");
                    var xElement = XElement.Load(fileName);
                    string defaultValue = "Select...";

                    foreach (var element in xElement.XPathSelectElements(string.Format(@"/df:Asset[df:ID={0}]/df:Attributes/df:Attribute", assetUid), xnm))
                    {
                        var name = element.Element(XName.Get("Name", xnm.LookupNamespace("df"))).Value;
                        var value = element.Element(XName.Get("Value", xnm.LookupNamespace("df")));
                        string strValue = string.Empty;
                        if (value == null) // value is dynamic list
                        {
                            var ds = element.Element(XName.Get("DynamicLists", xnm.LookupNamespace("df"))).Elements();
                            strValue = string.Join(
                                ", ",
                                ds.Select(e => e.Element(XName.Get(
                                    "Value",
                                    xnm.LookupNamespace("df"))
                                ).Value).Where(el => el != defaultValue).ToArray()
                            );
                        }
                        else
                        {
                            strValue = value.Value;
                        }

                        asset.Add(name, strValue);
                    }
                }
                catch (XmlException)
                {
                    throw;
                }
            }

            return asset;
        }

        /// <summary>
        /// Trying to find imported asset by barcode and return result as Status = OK - found, Error - Unknown asset
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <param name="otherAdapter">The other adapter to look for missed assets.</param>
        /// <returns>
        /// Return <see cref="LogRecordStatus"/> value
        /// </returns>
        public InventResult InventAsset(string barcode)
        {
            var row = this.AssetsTable.Select(string.Format(@"Barcode = '{0}'", barcode));
            InventResult res = new InventResult();
            res.Barcode = barcode;

            var logRow = this.LogTable.Select(string.Format("Barcode = '{0}'", barcode));
            if (logRow != null && logRow.Length != 0)       // means, what asset with given barcode already was scanned and exist in log
            {
                res.Duplicate = true;                       // exclude data from log
            }
            else
            {
                if (row != null && row.Length > 0)
                {
                    res.Name = row[0]["Name"].ToString();
                    res.Status = LogRecordStatus.OK;
                    res.AssetId = (long)row[0]["AssetId"];
                    res.AssetTypeId = (long)row[0]["AssetTypeId"];
                }
                else
                {
                    res.Name = "Not found";
                    res.Status = LogRecordStatus.Error;
                    res.AssetTypeId = 0;
                    res.AssetId = 0;
                }
            }

            return res;
        }

        internal void ProcessInventResult(InventResult res)
        {
            this.AssetSet.LogTable.Rows.Add(res.Name, res.Barcode, res.Status, DateTime.Now, res.AssetTypeId, res.AssetId, res.Message, res.CurrentLocation, res.CurrentLocationId);
        }

        /// <summary>
        /// Finishes the scan and builds the scan report.
        /// </summary>
        internal void FinishScan()
        {
            var expected = this.AssetsTable.Select().AsEnumerable();
            var found = this.LogTable.Select().AsEnumerable();    // select distinct values, miss doubles
            RowBarcodeComparer rowBarcodeComparer = new RowBarcodeComparer();
            var missed = expected.Except(found, rowBarcodeComparer);
            // var unknown = found.Except(expected, rowBarcodeComparer);

            this.reportList.Clear();

            foreach (var item in found)
            {
                this.reportList.Add(
                    item["Name"].ToString(),
                    item["Barcode"].ToString(),
                    (LogRecordStatus)item["Status"],
                    ReportRowActionId.Leave,
                    long.Parse(item["AssetTypeId"].ToString()),
                    long.Parse(item["AssetId"].ToString()),
                    item["CurrentLocation"].ToString(),
                    long.Parse(item["CurrentLocationId"].ToString())
                );
            }

            foreach (var item in missed)
            {
                this.reportList.Add(
                    item["Name"].ToString(),
                    item["Barcode"].ToString(),
                    LogRecordStatus.Warning,
                    ReportRowActionId.Miss,
                    long.Parse(item["AssetTypeId"].ToString()),
                    long.Parse(item["AssetId"].ToString()),
                    string.Empty,   // empty location for missed assets passed
                    0
                );
            }

            if (this.ReportListChanged != null) this.ReportListChanged.Invoke();

            this.isFinished = true;
            this.FinishTime = DateTime.Now;
        }

        /// <summary>
        /// Syncs report scan the with database. Sync include - change location of unexptected assets,
        /// moving missed to special location Missed
        /// </summary>
        internal void SyncWithDatabase()
        {
            var items = this.reportList.Where(e => e.Action != ReportRowActionId.Leave);
            int index = 0;

            this.SyncStart(this, new LengthlyOperationStartEventArgs() { StepsCount = items.Count() });
            var unitOfWork = new AppFramework.DataProxy.UnitOfWork();

            unitOfWork.RunInTransaction(delegate
            {
                foreach (var item in items) // select all report rows with action not Skip
                {
                    item.Sync();
                    this.SyncStep(this, new LengthlyOperationStepEventArgs() { Position = index });
                    index++;
                }
            });
            this.SyncEnd(this, null);
            this.IsSynchronized = true;
        }

        /// <summary>
        /// Clears the report list.
        /// </summary>
        internal void ClearReportList()
        {
            this.reportList.Clear();
            if (this.ReportListChanged != null) this.ReportListChanged.Invoke();
        }

        /// <summary>
        /// Saves the report as XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void SaveReportAsXml(Stream stream)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.reportList.GetType());
            xmlSerializer.Serialize(stream, this.reportList);
        }

        /// <summary>
        /// Moves the assets to given location
        /// </summary>
        /// <param name="locationId">The location id.</param>
        internal void MoveAssets(long locationId)
        {
            DataRow[] rows = LogTable.Select(string.Format("Status = {0}", (short)LogRecordStatus.OK));
            int count = rows.Length, index = 0;
            if (this.AssetMoveStart != null) this.AssetMoveStart.Invoke(this, new LengthlyOperationStartEventArgs() { StepsCount = count });
            foreach (DataRow row in rows)
            {
                Asset asset = SearchEngine.FindByBarcode(row["Barcode"].ToString()).FirstOrDefault();
                if (asset != null)
                {
                    asset["LocationId"].Value = locationId.ToString();
                    AssetFactory.InsertAsset(asset);
                    if (this.AssetMoved != null)
                    {
                        this.AssetMoved.Invoke(this, new LengthlyOperationStepEventArgs() { Position = index });
                        index++;
                    }
                }
            }
            if (this.AssetMoveEnd != null) this.AssetMoveEnd.Invoke(this, null);
        }

        internal void Preview()
        {
            int index = 0;
            this.PreviewStart.Invoke(this, new LengthlyOperationStartEventArgs() { StepsCount = this.ReportList.Count });
            var unitOfWork = new AppFramework.DataProxy.UnitOfWork();

            unitOfWork.RunInTransaction(delegate
            {
                foreach (ReportRow row in this.ReportList)
                {
                    row.Locate();
                    this.PreviewStep.Invoke(this, new LengthlyOperationStepEventArgs() { Position = index });
                    Application.DoEvents();
                    index++;
                }
            });
            this.IsPreviewed = true;
            this.PreviewEnd.Invoke(this, null);
        }   
    }
}