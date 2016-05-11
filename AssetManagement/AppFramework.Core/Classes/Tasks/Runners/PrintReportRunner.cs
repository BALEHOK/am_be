using AppFramework.Tasks;
using AppFramework.Tasks.Models;
using System.Collections.Generic;

namespace AppFramework.Core.Classes.Tasks.Runners
{
    public class PrintReportRunner : ITaskRunner
    {
        private string reportId;
        private long assetUid;
        private const string RENDER_URL = "~/Reports/RenderReport.aspx?Uid={0}&assetuid={1}";

        public PrintReportRunner(string reportId, long assetUid)
        {
            this.reportId = reportId;
            this.assetUid = assetUid;
        }

        public TaskResult Run(Entities.Task task)
        {
            var result = new TaskResult((Enumerations.TaskFunctionType)task.FunctionType);
            if (string.IsNullOrEmpty(reportId))
            {
                result.Status = Enumerations.TaskStatus.Error;
                result.NavigationResult = System.Web.VirtualPathUtility.ToAbsolute(
                    string.Format(RENDER_URL, reportId, assetUid));
                result.NavigationResultArguments = new Dictionary<string, object>
                {
                    { "reportId", reportId },
                    { "assetUid", assetUid }
                };
            }
            else
            {
                result.Status = Enumerations.TaskStatus.Sussess;
            }
            return result;
        }
    }
}
