using System.ServiceProcess;

namespace AssetManagementBatch
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new BatchService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
