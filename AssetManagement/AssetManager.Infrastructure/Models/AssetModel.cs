using System;
using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models
{
    public class AssetModel
    {
        public string Name { get; set; }

        public long AssetTypeId { get; set; }

        public long Id { get; set; }

        public int Revision { get; set; }

        public DateTime UpdatedAt { get; set; }

        public IEnumerable<TaxonomyModel> Taxonomy { get; set; }

        public IEnumerable<AssetScreenModel> Screens { get; set; }

        public bool Editable { get; set; }

        public bool Deletable { get; set; }

        public bool Reservable { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsHistory { get; set; }

        public string Barcode { get; set; }

        public IEnumerable<ChildAssetType> ChildAssetTypes { get; set; }
    }
}