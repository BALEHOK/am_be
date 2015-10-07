using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace AssetUpdater
{
    public class XmlDataBase: BaseXmlDataBase
    {

        public XmlDataBase(string path)
            : base(path)
        {
        }

        public string Version
        {
            get
            {
                return base.GetOption("Version");
            }

            set
            {
                base.SetOption("Version", value);
            }
        }

        public string WebsitePath
        {
            get { return base.GetOption("Path"); }
            set { base.SetOption("Path", value); }
        }
    }
}