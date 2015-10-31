using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Infrastructure.Models
{
    public class TaskResultModel : ResultBase<Dictionary<string, object>>
    {
        public string TaskFunctionType { get; set; }

        public bool ShouldRedirectOnComplete { get; set; }

        public string Status { get; set; }

        public string TaskName { get; set; }

        public long TaskId { get; set; }
    }
}
