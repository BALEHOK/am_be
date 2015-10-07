using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManager.Infrastructure.Models
{
    public class TaxonomyModel
    {
        public string Name { get; set; }

        public TaxonomyModel Child { get; set; }

        public AssetTypeModel AssetType { get; set; }
    }
}