using AppFramework.Tasks.Models;
using System.Collections.Generic;

namespace AppFramework.Tasks
{
    public class PredefinedTasks
    {
        public const string SobImportPayments = "SobImportPayments";

        public static List<PredefinedTaskModel> Tasks = new List<PredefinedTaskModel>(1)
            {
                new PredefinedTaskModel
                {
                    TaskId = SobImportPayments,
                    Name = "Import SOB Payments"
                },
            };
    }
}
