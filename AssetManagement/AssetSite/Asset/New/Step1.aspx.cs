using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace AssetSite.Asset.New
{
    public partial class Step1 : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["throw"] != null) throw new Exception("Error");

            if (!IsPostBack)
            {
                this.BindRecentData();
                this.BindAllData();
            }
        }

        private void BindAllData()
        {
            var aTypes = AssetTypeRepository.GetAllPublished().ToList();
            var data = new List<GridViewDataDescriptor>();

            foreach (var aType in aTypes)
            {
                var desc = new GridViewDataDescriptor(aType)
                {
                    Name = aType.Name, Description = aType.Comment
                };
                if (aType.ContextId.HasValue)
                {
                    var context = UnitOfWork.ContextRepository.SingleOrDefault(c => c.ContextId == aType.ContextId.Value);
                    desc.Context = context.Name;
                }

                var taxonomies = new List<AppFramework.Entities.TaxonomyItem>();
                var taxonomyItems = UnitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(aType.ID);
                foreach (var item in taxonomyItems)
                {
                    if (item.Taxonomy.IsActive && item.Taxonomy.ActiveVersion)
                        taxonomies.Add(item);
                }

                if (taxonomies.Count > 0)
                {
                    string path = this.GetParentName(taxonomies.First());

                    if (path.Contains("C>"))
                    {
                        path = path.Replace("C>", string.Empty);
                    }

                    IEnumerable<string> tempPath = path.Split(new char[1] { '>' }, StringSplitOptions.RemoveEmptyEntries).Reverse();
                    string nPath = string.Empty;
                    foreach (string pathElem in tempPath)
                    {
                        nPath += pathElem + " > ";
                    }
                    nPath = nPath.TrimEnd(new char[2] { '>', ' ' });
                    desc.Categories = nPath;
                }

                desc.DateRevision = aType.Revision.ToString("000") + " - " + aType.UpdateDate.ToString("dd-MM-yyyy");
                desc.ID = aType.ID;
                desc.AssetType = aType;

                data.Add(desc);
            }

            assetTypesGrid.DataSource = (from type in data
                                        where AuthenticationService.IsWritingAllowed(type.AssetType)
                                        select type).ToList();
            assetTypesGrid.DataBind();
        }

        private void BindRecentData()
        {
            var aTypes = AssetTypeRepository.GetRecent().ToList();
            var data = new List<GridViewDataDescriptor>();

            foreach (AssetType aType in aTypes)
            {
                var desc = new GridViewDataDescriptor(aType);
                desc.Name = aType.Name;
                desc.Description = aType.Comment;
                if (aType.ContextId.HasValue)
                {
                    var context = UnitOfWork.ContextRepository.SingleOrDefault(c => c.ContextId == aType.ContextId.Value);
                    desc.Context = context.Name;
                }

                var taxonomyItems = UnitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(aType.ID).ToList();
                if (taxonomyItems.Any())
                {
                    string path = this.GetParentName(taxonomyItems.First());

                    if (path.Contains("C>"))
                    {
                        path = path.Replace("C>", string.Empty);
                    }

                    IEnumerable<string> tempPath = path.Split(new char[1] { '>' }, StringSplitOptions.RemoveEmptyEntries).Reverse();
                    string nPath = string.Empty;
                    foreach (string pathElem in tempPath)
                    {
                        nPath += pathElem + " > ";
                    }
                    nPath = nPath.TrimEnd(new char[2] { '>', ' ' });
                    desc.Categories = nPath;
                }

                desc.DateRevision = aType.Revision.ToString("000") + " - " + aType.UpdateDate.ToString("dd-MM-yyyy");
                desc.ID = aType.ID;
                desc.AssetType = aType;

                data.Add(desc);
            }

            gvRecent.DataSource = (from type in data
                                   where AuthenticationService.IsWritingAllowed(type.AssetType)
                                   select type).ToList();
            gvRecent.DataBind();
        }

        private string GetParentName(AppFramework.Entities.TaxonomyItem item)
        {
            if (item.ParentItem == null)
            {
                if (item.Taxonomy.IsCategory)
                    return "C>" + item.Name;

                return item.Name;
            }

            if (item.Taxonomy.IsCategory)
                return "C>" + item.Name + ">" + this.GetParentName(item.ParentItem);

            return item.Name + ">" + this.GetParentName(item.ParentItem);
        }

        protected void assetTypesGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"]
                    = string.Format("location.href='/Asset/New/Step2.aspx?atid={0}'", (e.Row.DataItem as GridViewDataDescriptor).ID);
            }
        }

        protected void gvRecent_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"]
                    = string.Format("location.href='/Asset/New/Step2.aspx?atid={0}'", (e.Row.DataItem as GridViewDataDescriptor).ID);
            }
        }

        protected void assetTypesGrid_PageIndexChanged(object sender, EventArgs e)
        {
        }

        protected void assetTypesGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            assetTypesGrid.PageIndex = e.NewPageIndex;
            this.BindAllData();
            Response.Expires = -1;
        }
    }
}

