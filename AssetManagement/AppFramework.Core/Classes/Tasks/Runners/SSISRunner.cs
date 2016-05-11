namespace AppFramework.Core.Classes.Tasks.Runners
{
    using AppFramework.Tasks;
    using AppFramework.Tasks.Models;
    using Common.Logging;
    using Microsoft.SqlServer.Dts.Runtime;
    using System;
    using System.Collections.Generic;

    class SSISRunner : ITaskRunner
    {
        private ILog _logger = LogManager.GetCurrentClassLogger();

        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
        }

        private string _packageLocation;
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private List<string> _errors = new List<string>();
        private readonly TaskResult _result;

        public SSISRunner(string packageLocation)
        {
            if (string.IsNullOrEmpty(packageLocation))
                throw new ArgumentNullException();
            _packageLocation = packageLocation;
            _result = new TaskResult(Enumerations.TaskFunctionType.LaunchBatch);
        }

        public TaskResult Run(Entities.Task task)
        {
            SSISEventListener eventListener = new SSISEventListener(_result, _logger);
            Application app = new Application();
            try
            {
                Package pkg = app.LoadPackage(_packageLocation, eventListener);
                foreach (var param in Parameters)
                {
                    bool varAdded = false;
                    for (int i = 0; i < pkg.Variables.Count; i++)
                    {
                        if (pkg.Variables[i].Name == param.Key)
                        {
                            pkg.Variables[i].Value = param.Value;
                            varAdded = true;
                            break;
                        }
                    }

                    if (!varAdded)
                        pkg.Variables.Add(param.Key, true, "User", param.Value);
                }
                DTSExecResult pkgResults = pkg.Execute(null, null, eventListener, null, null);
                _result.Status = pkgResults == DTSExecResult.Success 
                    ? Enumerations.TaskStatus.Sussess 
                    : Enumerations.TaskStatus.Error;
            }
            catch (Exception ex)
            {
                _result.Errors.Add(ex.Message);
                _result.Status = Enumerations.TaskStatus.Error;
            }
            return _result;
        }

        private class SSISEventListener : DefaultEvents
        {
            private TaskResult _result;
            private ILog _logger;

            public SSISEventListener(TaskResult result, ILog logger)
                : base()
            {
                _result = result;
                _logger = logger;
            }

            public override bool OnError(DtsObject source, int errorCode, string subComponent,
              string description, string helpFile, int helpContext, string idofInterfaceWithError)
            {
                // Add application-specific diagnostics here.
                string message = string.Format("Error in {0}/{1} : {2}", source, subComponent, description);
                _logger.Error(message);
                _result.Errors.Add(message);
                return false;
            }
        }
    }
}
