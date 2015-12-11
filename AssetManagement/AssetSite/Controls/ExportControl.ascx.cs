using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetSite.Controls.SearchControls;
using AssetSite.Search;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using Microsoft.Practices.Unity;
using AssetManager.Infrastructure;
using AssetManager.Infrastructure.Services;

namespace AssetSite.Controls
{
    public partial class ExportControl : UserControl
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }
        [Dependency]
        public ITaxonomyItemService TaxonomyItemService { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public ISearchService SearchService { get; set; }

        private IEnumerable<IIndexEntity> _currentAssets { get; set; }
        private IExportService _exportService = new ExportService(new EnvironmentSettings());

        public event EventHandler ExportToTextClicked;
        public event EventHandler ExportToXmlClicked;
        public event EventHandler ExportToHtmlClicked;
        public event EventHandler ExportToExcelClicked;

        public void SetDataForExport(IEnumerable<IIndexEntity> entities)
        {
            _currentAssets = entities;
        }

        protected void lbtnExportToExcel_Click(object sender, EventArgs e)
        {
            if (Roles.IsUserInRole("Administrators") && ExportToExcelClicked != null)
                ExportToExcelClicked(sender, e);
        }

        protected void OnlbtnExportToTxt_Click(object sender, EventArgs e)
        {
            if (ExportToTextClicked != null)
                ExportToTextClicked(sender, e);
        }

        protected void OnlbtnExportToXml_Click(object sender, EventArgs e)
        {
            if (ExportToXmlClicked != null)
                ExportToXmlClicked(sender, e);
        }

        protected void lbtnExportToHtml_Click(object sender, EventArgs e)
        {
            if (ExportToHtmlClicked != null)
                ExportToHtmlClicked(sender, e);
        }

        public void ExportToText()
        {
            if (_currentAssets == null)
                throw new NullReferenceException("Export data not set");
            string content = _exportService.ExportSearchResultToTxt(_getEntities());
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "export" + Routines.SanitizeFileName(DateTime.Now.ToShortDateString()) + ".txt");
            Response.ContentType = "text/plain";
            Response.Write(content);
            Response.End();
        }

        public void ExportToXml()
        {
            if (_currentAssets == null)
                throw new NullReferenceException("Export data not set");
            string content = _exportService.ExportSearchResultToXml(_getEntities());
            Response.AddHeader("Content-Disposition", "attachment; filename=" + "export" + Routines.SanitizeFileName(DateTime.Now.ToShortDateString()) + ".xml");
            Response.ContentType = "text/xml";
            Response.Write(content);
            Response.End();
        }

        public void ExportToHtml()
        {
            if (_currentAssets == null)
                throw new NullReferenceException("Export data not set");

            StringBuilder sb = new StringBuilder();

            sb.Append(@"<html><head><meta http-equiv='content-type' content='text/html;charset=UTF-8'><style>");
            using (StreamReader rdr = new StreamReader(File.OpenRead(Server.MapPath("~/css/Search.css"))))
            {
                sb.Append(rdr.ReadToEnd());
            }
            sb.Append(@"</style></head><body>");

            SearchResult ctrl = (SearchResult)LoadControl("~/Controls/SearchControls/SearchResult.ascx");
            int i = 0;
            foreach (var item in _getEntities())
	        {
                sb.Append(_renderEntityToHtml(ctrl, item, i++ % 2 == 0));
	        }
            sb.Append("</body></html>");

            Response.AddHeader("Content-Disposition", "attachment; filename=" + "export" + DateTime.Now.ToShortTimeString().Replace(".", "_") + ".html");
            Response.AddHeader("Content-Length", sb.Length.ToString());
            Response.ContentType = "text/html";
            Response.Write(sb.ToString());
            Response.End();
        }

        public void ExportToExcel()
        {
            throw new NotImplementedException();
        }

        private string _renderEntityToHtml(SearchResult ctrl, IIndexEntity entity, bool isAlternate)
        {
            ctrl.BindEntity(entity, isAlternate);
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            ctrl.RenderControl(hw);
            return sb.ToString();
        }

        private IEnumerable<IIndexEntity> _getEntities()
        {
            List<IIndexEntity> result;
            var startRow = 1;
            var endRow = int.MaxValue;
            var pageType = Page.GetType().BaseType;

            var typeSearch = new TypeSearch(
                UnitOfWork,
                AssetTypeRepository,
                AssetsService);

            if (pageType == typeof (NewSimpleSearchKeywords))
            {
                // keywords
                var page = (Page as NewSimpleSearchKeywords);
                result = SearchService.FindByKeywords(
                    page.Params,
                    page.SearchId,
                    AuthenticationService.CurrentUserId,
                    page.ConfigsIds,
                    page.TaxonomyItemsIds,
                    page.Period,
                    page.OrderBy,
                    startRow,
                    endRow).ToList();
            }
            else if (pageType == typeof (NewResultByCategory))
            {
                // category
                var page = (Page as NewResultByCategory);
                result = SearchService.FindByKeywords(
                    page.Params,
                    page.SearchId,
                    AuthenticationService.CurrentUserId,
                    page.ConfigsIds,
                    page.TaxonomyItemsIds,
                    page.Period,
                    page.OrderBy,
                    startRow,
                    endRow).ToList();
            }
            else if (pageType == typeof (NewResultByType))
            {
                // type
                var page = (Page as NewResultByType);
                result = typeSearch.FindByType(
                    page.SearchId,
                    (long)AuthenticationService.CurrentUser.ProviderUserKey,
                    long.Parse(Request.QueryString["TypeUID"]),
                    (Session[Request.QueryString["Params"]] as List<AttributeElement>),
                    page.TaxonomyItemsIds,
                    page.Period,
                    page.OrderBy,
                    startRow,
                    endRow);
            }
            else
                throw new NotImplementedException();

            return result;
        }
    } 
}