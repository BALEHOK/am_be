using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssetUpdater.Code.Arguments;

namespace AssetUpdater.Code
{
    public delegate void SplashActionDoneDelegate(object sender, SplashActionDoneEventArgs e);

    public delegate void PackegProcessorProgressDelegate(object sender, PackageProcessorProgressEventArgs e);
}
