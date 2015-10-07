using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class AssetHistoryAttributeModel
    {
        public string NewValue { get; set; }

        public string OldValue { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public string DataType { get; set; }
    }
}