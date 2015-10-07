using System.Collections.Generic;
using System.Linq;

namespace AssetManager.Infrastructure.Models.TypeModels
{
    public class AssetTypeModel
    {
        public string DisplayName { get; set; }

        public string DbName { get; set; }

        public long Id { get; set; }

        public string Description { get; set; }

        public int Revision { get; set; }

        public System.DateTime UpdateDate { get; set; }

        public List<AttributeTypeModel> Attributes { get; set; }

        public List<AssetTypeScreenModel> Screens { get; set; }

        public bool HasScreenFormulas
        {
            get { return Attributes != null && Attributes.Any(a => a.HasScreenFormula); }
        }

        public bool HasCalculatedAttributes
        {
            get { return Attributes != null && Attributes.Any(a => a.HasDatabaseFormula); }
        }

        public bool HasValidationExpressions
        {
            get { return Attributes != null && Attributes.Any(a => a.HasValidationExpression); }
        }

        public bool IsHighlighted { get; set; }

        public AssetTypeModel()
        {
            Screens = new List<AssetTypeScreenModel>();
            Attributes = new List<AttributeTypeModel>();
        }
    }
}