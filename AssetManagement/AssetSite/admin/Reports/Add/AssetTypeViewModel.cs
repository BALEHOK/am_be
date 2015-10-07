using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes;

namespace AssetSite.admin.Reports.Add
{
    public class AssetTypeViewModel
    {
        private string name;
        private AppFramework.Core.Classes.AssetType assetType;

        public AssetTypeViewModel(string name, long attributeUid, AppFramework.Core.Classes.AssetType assetType)
        {
            // TODO: Complete member initialization
            this.name = name;
            this.assetType = assetType;
            this.attributeUid = attributeUid;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public AssetType AssetType
        {
            get
            {
                return this.assetType;
            }
        }

        private long attributeUid;
        public long AttributeUid
        {
            get { return this.attributeUid; }
        }

    }
}
