using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models
{
    public class TaskResultModel
    {
        public List<string> Errors { get; set; }

        public dynamic Result { get; set; }

        public string TaskFunctionType { get; set; }

        public bool ShouldRedirectOnComplete { get; set; }

        public string Status { get; set; }

        public string TaskName { get; set; }

        public object TaskId { get; set; }
    }
}
