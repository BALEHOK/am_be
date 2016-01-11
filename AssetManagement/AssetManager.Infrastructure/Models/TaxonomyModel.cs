using System.Collections.Generic;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManager.Infrastructure.Models
{
    public class TaxonomyModel
    {
        public IEnumerable<TaxonomyPathModel> TaxonomyPath { get; set; }

        public AssetTypeModel AssetType { get; set; }
    }
}