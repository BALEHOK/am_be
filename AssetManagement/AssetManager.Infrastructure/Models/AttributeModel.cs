using AssetManager.Infrastructure.Validators;
using FluentValidation.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

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

        public JToken Value { get; set; }

        public bool Required { get; set; }
    }
}