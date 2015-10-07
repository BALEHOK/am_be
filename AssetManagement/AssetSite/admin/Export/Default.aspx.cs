using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.IE.Providers;
using System;
using System.Data;
using System.Globalization;
using System.IO;


namespace AssetSite.admin.Export
{
    public partial class Default : BasePage
    {
        protected void GetAssets_Click(object sender, EventArgs e)
        {
            var exporter = new AssetsExporter(AssetTypeRepository, UnitOfWork, AssetsService);
            var dataTable = exporter.ExportToDataTable(exportParameters.AssetUID, exportParameters.SearchAttributes);
            var selectedDataSource = Routines.StringToEnum<DataSourceType>(dataSourceTypesList.SelectedValue);

            string filename = string.Format("export-{0}-{1}", 
                exportParameters.AssetUID,
                DateTime.Now.ToString("yyyyMMddhhmmss"));
            switch (selectedDataSource)
            {
                case DataSourceType.XML:
                    filename = filename + ".xml";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition",
                        string.Format("attachment; filename={0}", filename));
                    Response.ContentType = "application/xml";
                    dataTable.TableName = "Assets";
                    dataTable.WriteXml(Response.OutputStream);
                    Response.End(); 
                    break;

                case DataSourceType.XLS:
                    filename = filename + ".xlsx";
                    var filepath = Path.Combine(ApplicationSettings.TempFullPath, filename);
                    _datatableToXlsxSchema(dataTable, filepath);
                    var stream = File.OpenRead(filepath);
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                    File.Delete(filepath);

                    Response.Clear();
                    Response.ContentType = "application/x-msexcel";
                    Response.AddHeader("Content-Length", buffer.Length.ToString(CultureInfo.InvariantCulture));
                    Response.AddHeader("Content-Disposition",
                        string.Format("attachment;filename={0}", filename));
                    Response.BinaryWrite(buffer);
                    Response.End();
                    break;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("/admin");
        }

        private void _datatableToXlsxSchema(DataTable dt, string filename)
        {
            dt.TableName = "Sheet1$";
            var xlsProvider = new ExcelProvider();
            xlsProvider.WriteWithData(dt, filename);
        } 
    }
}
