using AppFramework.Core.Classes;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AssetSite.admin.Taxonomies
{
    public partial class Default : BasePage
    {
        private readonly ITaxonomyService _taxonomyService;

        public Default()
        {
            _taxonomyService = new TaxonomyService(UnitOfWork);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackClientHyperlink(new Control(), String.Empty);   // force __doPostback() generation

            Session["Taxonomy"] = null;

            if (IsPostBack)
            {
                string PostBackerID = Request.Form.Get(Page.postEventSourceID);
                string PostBackerArg = Request.Form.Get(Page.postEventArgumentID);
                if (PostBackerID == "CategorySelector")
                {
                    long uid = 0;
                    if (long.TryParse(PostBackerArg, out uid) && uid > 0)
                    {
                        _taxonomyService.SetCategoryByUid(uid);
                        Response.Redirect(Request.Url.OriginalString);
                    }
                }
            }
        }

        protected void AddTaxonomy(object sender, EventArgs e)
        {
            _taxonomyService.Save(new AppFramework.Entities.Taxonomy
            {
                Name = TaxText.Text,
                Description = TaxDescr.Text,
                IsCategory = false,
                IsDraft = true
            },
            AuthenticationService.CurrentUserId);
            Response.Redirect(Request.Url.OriginalString);
        }

        public string IsCategoryChecked(object boolVal)
        {
            return (bool)boolVal == true ? "checked" : "";
        }

        protected void TaxonomiesGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = e.Row.DataItem as AppFramework.Entities.Taxonomy;
                if (item != null && item.IsDraft)
                {
                    HyperLink linkField = e.Row.Cells[e.Row.Cells.Count - 3].Controls[0] as HyperLink;
                    linkField.Text = Resources.Global.DraftText;
                    linkField.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void Taxonomy_Deleting(object sender, EntityDataSourceChangingEventArgs args)
        {
            var entity = args.Entity as AppFramework.Entities.Taxonomy;
            _taxonomyService.Delete(entity);
            args.Cancel = true;
        }

        protected void Taxonomy_Updating(object sender, EntityDataSourceChangingEventArgs args)
        {
            var entity = args.Entity as AppFramework.Entities.Taxonomy;
            entity.UpdateDate = DateTime.Now;
            entity.UpdateUserId = AuthenticationService.CurrentUserId;
        }

        protected string GetTaxonomyName(object dataItem)
        {
            return new TranslatableString((string)DataBinder.Eval(dataItem, "Name")).GetTranslation();
        }
    }
}
