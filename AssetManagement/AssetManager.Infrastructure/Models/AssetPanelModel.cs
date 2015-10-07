using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class AssetPanelModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<AttributeModel> Attributes { get; set; }
    }
}