using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class AttributeValidationResultModel
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
}