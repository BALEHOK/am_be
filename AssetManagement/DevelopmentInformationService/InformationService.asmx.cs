using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace DevelopmentInformationService
{
    /// <summary>
    /// Summary description for InformationService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class InformationService : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetVersion()
        {
            XmlDataBase _base = new XmlDataBase(Server.MapPath("~/Account/settings.xml"));
            return _base.Version;
        }

        // file format version_update.sql
        [WebMethod]
        public FileDescriptor GetScripts(string version)
        {
            string path = Server.MapPath("~/SQLPackage/");
            if (!Directory.Exists(path))
                return null;

            string filePath = Path.Combine(path, version + "_update.zip");
            if (!File.Exists(filePath))
                return null;

            FileInfo fi = new FileInfo(filePath);

            FileDescriptor desc = new FileDescriptor();
            desc.Url = this.Context.Request.Url.AbsoluteUri.Replace(this.Context.Request.Url.AbsolutePath, "") + "/SQLPackage/" + version + "_update.zip";
            desc.Length = fi.Length;

            return desc;
        }

        [WebMethod]
        public FileDescriptor GetPackage(string version)
        {
            string path = Server.MapPath("~/AppPackage/");
            if (!Directory.Exists(path))
                return null;

            string filePath = Path.Combine(path, version + "_update.zip");
            if (!File.Exists(filePath))
                return null;

            FileInfo fi = new FileInfo(filePath);

            FileDescriptor desc = new FileDescriptor();
            desc.Url = this.Context.Request.Url.AbsoluteUri.Replace(this.Context.Request.Url.AbsolutePath, "") + "/AppPackage/" + version + "_update.zip";
            desc.Length = fi.Length;

            return desc;
        }
    }
}
