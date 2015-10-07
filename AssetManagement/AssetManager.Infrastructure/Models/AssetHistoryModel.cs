using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class AssetHistoryModel
    {
        public List<AssetRevisionModel> Revisions { get; set; }
    }
}