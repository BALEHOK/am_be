using AppFramework.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class AssetAttributeRelatedEntitiesModel
    {
        public long AttributeUid { get; set; }

        public long AttributeId { get; set; }

        public string Name { get; set; }

        public string Datatype { get; set; }

        public long? RelatedAssetTypeId { get; set; }

        public IEnumerable<PlainAssetDTO> Assets { get; set; }

        public DynListModel List { get; set; }
    }
}