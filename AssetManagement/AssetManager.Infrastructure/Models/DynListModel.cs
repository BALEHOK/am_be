using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class DynListModel
    {
        public long Uid { get; set; }

        public string Name { get; set; }

        public IEnumerable<DynListItemModel> Items { get; set; }
    }
}