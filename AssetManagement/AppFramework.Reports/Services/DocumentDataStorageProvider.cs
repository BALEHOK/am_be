using DevExpress.Xpf.Printing.Service.Extensions;
using System;
using System.ComponentModel.Composition;

namespace AppFramework.Reports.Services
{
    [Export(typeof(IDocumentDataStorageProvider))]
    public class DocumentDataStorageProvider : IDocumentDataStorageProvider
    {
        string IDocumentDataStorageProvider.ConnectionString
        {
            get
            {
                //var configurationConnectionString = ConfigurationManager.ConnectionStrings["xpf.printing"];
                //if (configurationConnectionString == null)
                //{
                //    return null;
                //}
                //return DbEngineDetector.PatchConnectionString(configurationConnectionString.ConnectionString);
                //"xpoprovider=MSSqlServer;data source=(localdb)\\v11.0;attachdbfilename=|DataDirectory|\\ReportService.mdf;integrated security=True;connect timeout=120";
                return "XpoProvider=MSAccess;Provider=Microsoft.Jet.OLEDB.4.0;Mode=Share Deny None;data source=|DataDirectory|/ReportService.mdb;user id=Admin;password=;";
            }
        }

        TimeSpan IDocumentDataStorageProvider.KeepInterval
        {
            get { return this.GetDefaultKeepInterval(); }
        }

        int IDocumentDataStorageProvider.BinaryStorageChunkSize
        {
            get { return this.GetDefaultBinaryStorageChunkSize(); }
        }
    }
}
