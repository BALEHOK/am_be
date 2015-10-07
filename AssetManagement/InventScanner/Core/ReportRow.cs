/*--------------------------------------------------------
* ReportRow.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/12/2009 6:41:22 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using AppFramework.Core.Classes.Stock;
    using AppFramework.Core.Classes;
    //using AppFramework.DynamicEntity.Services;
    using AppFramework.Core.Classes.SearchEngine;

    internal struct LocationInfo
    {
        public Asset Asset;
        public string Name;
        public long Id;
    }

    [DataObject]
    public class ReportRow
    {
        [DataObjectField(false)]
        public string Name { get; set; }

        [DataObjectField(true)]
        public string Barcode { get; set; }

        [DataObjectField(false)]
        public LogRecordStatus Status { get; set; }

        [DataObjectField(false)]
        public ReportRowActionId Action { get; set; }

        [DataObjectField(false)]
        public string SyncMessage { get; set; }

        [DataObjectField(false)]
        public string CurrentLocation { get; set; }

        public long CurrentLocationId { get; set; }

        public long AssetTypeId { get; set; }

        public long AssetId { get; set; }

        public long LocationId { get; set; }

        public string LocationName { get; set; }

        public long LocationOut { get; set; }

        public bool Selected { get; set; }

        /// <summary>
        /// Syncs this instance.
        /// </summary>
        internal void Sync()
        {
            if (this.Status != LogRecordStatus.Error)
            {
                switch (this.Action)
                {
                    case ReportRowActionId.Leave:
                        break;

                    case ReportRowActionId.Move:
                        this.Move();
                        break;

                    case ReportRowActionId.Miss:
                        this.MoveAssetToMissedLocation();
                        break;

                    default:
                        break;
                }
            }
            else
            {
                this.SyncMessage = "Asset not found in database, unable to perform any action";
            }
        }

        private void Move()
        {
            AssetType locationAssetType = AssetType.GetByID(PredefinedAttribute.Get(AppFramework.ConstantsEnumerators.PredefinedEntity.Location).DynEntityConfigID);
            Asset location = AssetFactory.GetAssetById(locationAssetType, this.LocationOut);
            if (location != null)
            {
                this.MoveAssetToLocation(this.LocationOut);
                this.SyncMessage = string.Format("Asset moved to '{0}' location", location.Name);
                this.Status = LogRecordStatus.OK;
                this.CurrentLocationId = location.ID;
                this.CurrentLocation = location.Name;
            }
            else
            {
                this.Status = LogRecordStatus.Error;
                this.SyncMessage = "Sync failed, location not found";
            }
        }

        /// <summary>
        /// Moves the asset to missed location - from predefined assets
        /// </summary>
        private void MoveAssetToMissedLocation()
        {
            var missedLocation = AppFramework.Core.Classes.PredefinedAsset.MissedLocation;
            if (missedLocation != null)
            {
                this.MoveAssetToLocation(missedLocation.ID);
                this.SyncMessage = "Asset moved to 'Missed' location";
                this.Status = LogRecordStatus.OK;
                this.Action = ReportRowActionId.Leave;
                this.CurrentLocation = "Missed";
                this.CurrentLocationId = missedLocation.ID;
            }
            else
            {
                this.SyncMessage = "'Missed' location not configired, unable to move";
                this.Status = LogRecordStatus.Error;
            }
        }

        /// <summary>
        /// Moves the asset to location.
        /// </summary>
        /// <param name="locationId">The location id.</param>
        private void MoveAssetToLocation(long locationId)
        {
            AssetType at = AssetType.GetByID(this.AssetTypeId);
            Asset thisAsset = AssetFactory.GetAssetById(at, this.AssetId);
            if (thisAsset != null)
            {
                thisAsset["LocationId"].Value = locationId.ToString();
            }
            AssetFactory.InsertAsset(thisAsset);
        }

        internal void Locate()
        {
            // data about asset location from database
            LocationInfo info = this.FindInDatabase(this.Barcode);

            switch (this.Status)
            {
                // OK status means what asset expected in this location and found here
                case LogRecordStatus.OK:
                    // asset was not moved
                    if (info.Id == this.LocationId)
                    {
                        this.CurrentLocation = this.LocationName;
                        this.CurrentLocationId = this.LocationId;
                    }
                    else
                    {
                        this.CurrentLocation = info.Name;
                        this.CurrentLocationId = info.Id;
                        this.Status = LogRecordStatus.Warning;
                        this.Action = ReportRowActionId.Move;
                        this.LocationOut = this.LocationId;
                    }
                    break;

                // Warning - asset exist in list of exported, but not found in location / found in another exported location
                case LogRecordStatus.Warning:
                    // if this asset found in another exported location
                    if (this.CurrentLocationId != 0)
                    {
                        if (this.CurrentLocationId != info.Id)
                        {
                            this.CurrentLocationId = info.Id;
                            this.CurrentLocation = info.Name;
                        }
                        this.Action = ReportRowActionId.Move;
                        this.LocationOut = this.LocationId;
                    }
                    else
                    {
                        // asset found in database but in another location
                        if (info.Id != 0 && this.LocationId != info.Id)
                        {
                            this.CurrentLocationId = info.Id;
                            this.CurrentLocation = info.Name;
                            this.Action = ReportRowActionId.Leave;
                        }
                        else
                        {
                            // asset found in database in this location, but was not found during scan
                            this.Action = ReportRowActionId.Miss;
                        }
                    }
                    break;

                // Error - asset unknow. Possible, it located in not exported location or it not registered in system
                case LogRecordStatus.Error:
                    // asset located in other location in database?
                    if (info.Id != 0)
                    {
                        this.Name = info.Asset.Name;
                        this.CurrentLocationId = info.Id;
                        this.CurrentLocation = info.Name;
                        this.AssetId = info.Asset.ID;
                        this.AssetTypeId = info.Asset.GetConfiguration().ID;
                        this.Status = LogRecordStatus.Warning;

                        if (this.LocationId != info.Id)
                        {
                            this.Action = ReportRowActionId.Move;
                            this.LocationOut = this.LocationId;
                        }
                        else
                        {
                            this.Action = ReportRowActionId.Miss;
                        }
                    }
                    else
                    {
                        this.Action = ReportRowActionId.Leave;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Finds the asset in database and returns location information
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Return <see cref="LocationInfo"/> value</returns>
        private LocationInfo FindInDatabase(string barcode)
        {
            LocationInfo info = new LocationInfo();
            Asset asset = SearchEngine.FindByBarcode(barcode).FirstOrDefault();
            if (asset != null)
            {
                info.Asset = asset;
                Asset location = asset["LocationId"].RelatedAsset;
                if (location != null)
                {
                    info.Id = location.ID;
                    info.Name = location.Name;
                }
            }
            return info;
        }
    }
}
