using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace AssetSite
{
    /// <summary>
    /// Summary description for DownloadHandler
    /// </summary>
    public class DownloadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            // TODO: remove that class
            throw new NotSupportedException("Seems line an obsolete code.");

            if (context.Request["file"] != null)
            {
                string filename = context.Request["file"].ToString();
                string fullpath = context.Server.MapPath(string.Format("~/App_Data/{0}", filename));
                if (!File.Exists(fullpath))
                {
                    context.Response.Write("File not found.");
                    context.Response.End();
                    return;
                }
                string contentType = "text/plain";
                switch (filename.Split(new char[] { '.' }).Last().ToLower())
                {
                    case "xls":
                    case "xslx":
                        contentType = "application/ms-excel";
                        break;
                    case "csv":
                        contentType = "text/csv";
                        break;
                    default:
                        break;
                }
                string fileContent = File.ReadAllText(fullpath);
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                context.Response.AddHeader("Content-Length", fileContent.Length.ToString());
                context.Response.ContentType = contentType;
                context.Response.Write(fileContent);
                context.Response.End();
            }
            else
            {
                context.Response.Write("File parameter missing.");
                context.Response.End();
                return;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}