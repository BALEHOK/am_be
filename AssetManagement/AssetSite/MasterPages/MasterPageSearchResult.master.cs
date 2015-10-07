using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Batch;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Entities;
using AppFramework.Reports;

namespace AssetSite.MasterPages
{
    public partial class MasterPageSearchResult : System.Web.UI.MasterPage
    {
        public int SearchId
        {
            get
            {
                if (Request["SearchId"] != null)
                    return int.Parse(Request["SearchId"]);
                else return default(int);
            }
        }

        public Dictionary<int, List<IIndexEntity>> ResultSet
        {
            get
            {
                if (Session["InitialResult"] == null)
                {
                    Session["InitialResult"] = new Dictionary<int, List<IIndexEntity>>();
                }
                return Session["InitialResult"] as Dictionary<int, List<IIndexEntity>>;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ExportControl1.ExportToTextClicked += new EventHandler(ExportControl1_ExportToTextClicked);
            ExportControl1.ExportToXmlClicked += new EventHandler(ExportControl1_ExportToXmlClicked);
            ExportControl1.ExportToHtmlClicked += new EventHandler(ExportControl1_ExportToHtmlClicked);
            ExportControl1.ExportToExcelClicked += new EventHandler(ExportControl1_ExportToExcelClicked);
        }

        void ExportControl1_ExportToExcelClicked(object sender, EventArgs e)
        {
            ExportControl1.SetDataForExport(ResultSet[SearchId]);
            ExportControl1.ExportToExcel();
        }

        void ExportControl1_ExportToHtmlClicked(object sender, EventArgs e)
        {
            ExportControl1.SetDataForExport(ResultSet[SearchId]);
            ExportControl1.ExportToHtml();
        }

        void ExportControl1_ExportToXmlClicked(object sender, EventArgs e)
        {
            ExportControl1.SetDataForExport(ResultSet[SearchId]);
            ExportControl1.ExportToXml();
        }

        void ExportControl1_ExportToTextClicked(object sender, EventArgs e)
        {
            ExportControl1.SetDataForExport(ResultSet[SearchId]);
            ExportControl1.ExportToText();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ResultSet != null && ResultSet.ContainsKey(SearchId))
                {
                    lvResults.DataSource = ResultSet[SearchId];
                    lvResults.DataBind();                    
                }
            }
        }
    }
}