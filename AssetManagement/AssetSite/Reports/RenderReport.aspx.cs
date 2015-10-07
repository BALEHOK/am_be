using System.Collections.Generic;
using AppFramework.Entities;

namespace AssetSite.Reports
{
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;
    using AppFramework.Core.Classes.Reports;
    using AssetSite.admin.Reports;

    public partial class RenderReport : ReportPage
    {
        private string ASSETUID_PARAM = "assetuid";
		private string SEARCH_PARAM = "fromSearch";

        protected void Page_Load(object sender, EventArgs e)
        {
            InitPage();

            var q = this.report.Fields.Where(f => f.IsFilter && f.IsVisible);
            if (q.Count() != 0 && (!this.report.SyncWithItem || !this.report.AssetUid.HasValue))
            {
                FilterValues.DataSource = this.report.Fields.Where(f => f.IsFilter && f.IsVisible);
                FilterValues.DataBind();
            }
            else
            {
                ViewReport();
            }
        }

        private void InitPage()
        {
            var queryStringValue = Request.QueryString[ASSETUID_PARAM];
            if (!string.IsNullOrEmpty(queryStringValue))
            {
                long value = 0;
                if (long.TryParse(queryStringValue, out value))
                {
                    this.report.AssetUid = value;
                }
            }

        	var isFromSearchParam = Request.QueryString[SEARCH_PARAM];
        	bool isFromSearch;
			if(bool.TryParse(isFromSearchParam, out isFromSearch))
			{
				if(isFromSearch)
				{
					report.EntityIndicesFromSearch = Session["InitialResult"] as IEnumerable<IIndexEntity>;
				}
			}
        }

        protected void FilterValuesRowDataBound(object sender, GridViewRowEventArgs e)
        {
            ReportField rf = e.Row.DataItem as ReportField;
            if (rf != null)
            {
                e.Row.Cells[1].Controls.Add(rf.FilterControl);
            }
        }

        /// <summary>
        /// Gets the filtered report.
        /// </summary>
        /// <returns></returns>
        public Report GetFilteredReport()
        {
            foreach (GridViewRow item in FilterValues.Rows)
            {
                string name = (item.Cells[0] as DataControlFieldCell).Text;
                string value = (item.Cells[1].Controls[0] as IFilterControl).GetValue();
                string text = (item.Cells[1].Controls[0] as IFilterControl).GetText();

                this.report.SetFilterValue(name, value, text);
            }

			return this.report;
        }

        protected void ViewReportClick(object sender, EventArgs e)
        {
            ViewReport();
        }

        private void ViewReport()
        {
            Session["Report"] = this.GetFilteredReport();
            Response.Redirect("ViewReport.aspx");
        }
    }
}
