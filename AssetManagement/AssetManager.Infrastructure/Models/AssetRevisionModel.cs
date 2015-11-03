using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class AssetRevisionModel
    {
        public long AssetId { get; set; }

        public long AssetTypeId { get; set; }

        public string RevisionNumber { get; set; }

        public string CreatedAt { get; set; }

        public List<AssetHistoryAttributeModel> ChangedValues { get; set; }

        public AppFramework.Core.DTO.PlainAssetDTO CreatedByUser { get; set; }
    }
}