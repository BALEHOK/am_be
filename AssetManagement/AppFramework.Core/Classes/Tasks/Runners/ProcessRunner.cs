namespace AppFramework.Core.Classes.Tasks.Runners
{
    using AppFramework.Tasks;
    using AppFramework.Tasks.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    class ProcessRunner : ITaskRunner
    {
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
        }
        public string ExecutablePath { get; set; }

        public TaskResult Run(Entities.Task task)
        {
            // Create the ProcessInfo object
            ProcessStartInfo psi = new ProcessStartInfo(ExecutablePath);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;

            if (Parameters.Count > 0)
            {
                string arguments = string.Empty;
                foreach (var key in Parameters.Keys)
                {
                    arguments += "-" + key + (!string.IsNullOrEmpty(Parameters[key]) ? (" \"" + Parameters[key] + "\" ") : " ");
                }
                psi.Arguments = arguments;
            }

            Process proc = null;
            var result = new TaskResult((Enumerations.TaskFunctionType)task.FunctionType);
            try
            {
                // Start the process
                proc = Process.Start(psi);
                result.Status = Enumerations.TaskStatus.Sussess;
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
                result.Status = Enumerations.TaskStatus.Error;
            }
            finally
            {
                if (proc != null)
                    proc.Close();
            }
            return result;
        }
    }
}
