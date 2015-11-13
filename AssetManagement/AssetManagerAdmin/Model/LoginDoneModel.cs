using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagerAdmin.Model
{
    public class LoginDoneModel
    {
        public ServerConfig Server { get; internal set; }
        public UserInfo User { get; internal set; }
    }
}
