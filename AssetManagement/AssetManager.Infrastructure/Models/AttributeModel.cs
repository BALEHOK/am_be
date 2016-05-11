using AssetManager.Infrastructure.Validators;
using FluentValidation.Attributes;
using Newtonsoft.Json.Linq;

namespace AssetManager.Infrastructure.Models
{
    [Validator(typeof(AttributeModelValidator))]
    public class AttributeModel
    {
        public long Uid { get; set; }

        public long Id { get; set; }

        public long? RelatedAssetTypeId { get; set; }

        public string Name { get; set; }

        public string Datatype { get; set; }

        public bool Editable { get; set; }

        public bool CanCreateNew { get; set; }

        public JToken Value { get; set; }

        public bool Required { get; set; }

        public long AssetTypeId { get; internal set; }
    }
}