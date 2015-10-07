using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using System;
using System.Data;
using System.Text;
using System.Web;

namespace AppFramework.Core.Classes.Tasks.Runners
{
    class ExportRunner : SearchRunner
    {
        public override TaskResult Run(Entities.Task task)
        {
            int totalItems = 0;
            SearchOutput result;

            switch (this.Params.SearchType)
            {
                case SearchType.SearchByKeywords:
                    // TODO
                    throw new NotImplementedException();
                    //result = SearchEngine.SearchEngine.FindByKeywords(this.Params.Keywords, this.Params.TimePeriod, out totalItems, true);
                    break;

                case SearchType.SearchByCategory:
                    // TODO
                    throw new NotImplementedException();
                    //result = SearchEngine.SearchEngine.FindByKeywords(this.Params.TaxonomyItems, this.Params.Keywords,
                    //    this.Params.TimePeriod, 0, int.MaxValue, out totalItems, true);
                    break;

                case SearchType.SearchByContext:
                    // TODO
                    throw new NotImplementedException();
                    //result = SearchEngine.SearchEngine.FindByContext(this.Params.Elements, this.Params.TimePeriod, 0,
                    //    int.MaxValue, out totalItems, true);
                    break;

                case SearchType.SearchByDocuments:
                    // TODO
                    throw new NotImplementedException();
                    //result = SearchEngine.SearchEngine.FindDocumentsByKeywords(this.Params.Keywords, out totalItems, true);
                    break;

                case SearchType.SearchByType:
                    // TODO
                    throw new NotImplementedException();
                    //result = SearchEngine.SearchEngine.FindByAssetType(this.Params.AssetTypeUID, this.Params.Elements, this.Params.TimePeriod,
                    //    out totalItems, null, null, true);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            DataTable dt = new DataTable("Export$");
            dt.Columns.AddRange(new DataColumn[]{
                new DataColumn("AssetTypeId"),
                new DataColumn("AssetId"),
                new DataColumn("Name"),
                new DataColumn("ShortDetails"),
                new DataColumn("ExtendedDetails"),
                new DataColumn("Location"),
                new DataColumn("Department"),
                new DataColumn("UpdateDate"),
                new DataColumn("User"),
            });

            foreach (var entity in result.Entities)
            {
                dt.Rows.Add(new object[]{
                    entity.DynEntityConfigId,
                    entity.DynEntityId,
                    entity.Name,
                    entity.DisplayValues,
                    entity.DisplayExtValues,
                    entity.Location,
                    entity.Department,
                    entity.UpdateDate,
                    entity.User
                });
            }

            var taskResult = new TaskResult((TaskFunctionType)task.FunctionType);

            try
            {
                string filename = string.Format("{0}_export.csv", DateTime.Now.ToString("yyyyMMdd"));
                string filePath = HttpContext.Current.Server.MapPath("~/App_Data/") + filename;
                System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false, Encoding.UTF8);

                foreach (DataColumn column in dt.Columns)
                {
                    file.Write(column.Caption + ";");
                }
                file.WriteLine();
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        file.Write(row[column].ToString().Replace(";", string.Empty) + ";");
                    }
                    file.WriteLine();
                }
                file.Close();

                taskResult.Status = TaskStatus.Sussess;
                taskResult.NavigationResult = 
                    HttpContext.Current.Request.Url.Scheme + "://" +
                    HttpContext.Current.Request.Url.Authority + "/DownloadHandler.ashx?file=" + filename;
            }
            catch(Exception ex)
            {
                taskResult.Status = TaskStatus.Error;
                taskResult.Errors.Add(ex.Message);
            }
            return taskResult;
        }
    }
}
