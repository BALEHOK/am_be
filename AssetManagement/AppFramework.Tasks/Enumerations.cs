namespace AppFramework.Tasks
{
    public class Enumerations
    {
        public enum TaskStatus
        {
            Sussess = 0,
            Error = 1
        }

        public enum TaskExecutableType
        {
            Internal = 0,
            Assembly = 1,
            SSIS = 2,
            PredefinedTask = 3,
        }

        public enum TaskFunctionType
        {
            ExecuteSearch = 0,
            LaunchBatch = 1,
            ImportFile = 2,
            CreateAsset = 4,
            PrintReport = 5,
            ExportFileSSIS = 6,
            ExecuteSqlServerAgentJob = 7,
            Other = 999
        }

        public enum TaskImportFileType
        {
            Excel,
            XML
        }

        public enum TaskActionOnComplete
        {
            Navigate = 0,
            Nothing = 1
        }
    }
}
