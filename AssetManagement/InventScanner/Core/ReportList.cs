/*--------------------------------------------------------
* ReportList.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/12/2009 5:16:34 PM
* Purpose: ReportList based on List instead DataTable because better performance
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AppFramework.Core.Classes.Stock;

    /// <summary>
    /// 
    /// </summary>
    public class ReportList : List<ReportRow>
    {
        private XmlAdapter xmlAdapter;

        public ReportList(XmlAdapter adapter)
        {
            this.xmlAdapter = adapter;
        }

        public void Add(string name, string barcode, LogRecordStatus status, ReportRowActionId action)            
        {
            this.Add(name, barcode, status, action, 0, 0, "", 0 );
        }

        public void Add(string name, string barcode, LogRecordStatus status, ReportRowActionId action, long assetTypeId, long assetId, string currentLocation, long currentLocationId)
        {
            var r = new ReportRow()
            {
                Name = name,
                Status = status,
                Barcode = barcode,
                Action = action,
                AssetTypeId = assetTypeId,
                AssetId = assetId,
                LocationId = xmlAdapter.LocationId,
                LocationName = xmlAdapter.LocationName,
                CurrentLocation = currentLocation,
                CurrentLocationId = currentLocationId,
                LocationOut = this.xmlAdapter.LocationId
            };

            this.Add(r);
        }
    }
}
