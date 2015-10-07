using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetUpdater.AssetManagementInformationService;

namespace AssetUpdater.Code
{
    public enum ListItemState
    {
        Downloading,
        Ok,
        Error
    }

    public class BGWorkerArgument : FileDescriptor
    {
        public bool IsSQL { get; set; }

        public BGWorkerArgument()
            : base()
        {
        }

        public BGWorkerArgument(FileDescriptor desc)
        {
            this.Url = desc.Url;
            this.Length = desc.Length;
            this.IsSQL = false;
        }
    }
}
