using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Infrastructure.Models
{
    public class CustomReportModel
    {
        public long Id { get; set; }
        public long AssetTypeId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
    }
}