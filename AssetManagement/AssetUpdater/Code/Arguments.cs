using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetUpdater.Code.Arguments
{
    public class SplashActionDoneEventArgs : EventArgs
    {
        public object Info { get; set; }

        public SplashActionDoneEventArgs() : base() { }
    }

    public class PackageProcessorProgressEventArgs : EventArgs
    {
        public string Info { get; set; }
        public long Progress { get; set; }
        public string ActionID { get; set; }

        public PackageProcessorProgressEventArgs() : base() 
        {
            this.Progress = 0;
            this.ActionID = string.Empty;
        }
    }
}
