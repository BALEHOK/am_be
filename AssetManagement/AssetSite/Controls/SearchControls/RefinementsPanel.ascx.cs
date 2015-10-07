using System.Web.UI;
using AppFramework.Core.Classes;
using AssetSite.Search;
using System;
using System.Linq;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls.SearchControls
{
    public partial class RefinementsPanel : UserControl
    {
        [Dependency]
        public ITaxonomyItemService TaxonomyItemService { get; set; }

        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
      
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            
            var page = (Page as SearchResultPage);
            if (!string.IsNullOrEmpty(page.ConfigsIds))
            {
                Visible = true;
                AssetTypes.DataSource = from id in page.ConfigsIds.Split(new char[] { ',', ' ' })
                                        where !string.IsNullOrEmpty(id)
                                        select AssetTypeRepository.GetById(long.Parse(id));
                AssetTypes.DataBind();
            }

            if (!string.IsNullOrEmpty(page.TaxonomyItemsIds))
            {
                Visible = true;
                Taxonomies.DataSource = from id in page.TaxonomyItemsIds.Split(new char[] { ',', ' ' })
                                        where !string.IsNullOrEmpty(id)
                                        select TaxonomyItemService.GetActiveItemById(long.Parse(id));
                Taxonomies.DataBind();
            }
        }
    }
}