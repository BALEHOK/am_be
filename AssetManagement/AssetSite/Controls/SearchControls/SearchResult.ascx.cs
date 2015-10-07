namespace AssetSite.Controls.SearchControls
{
    using System;
    using System.Web.UI;
    using AppFramework.Entities;

    public partial class SearchResult : UserControl
    {
        public IIndexEntity Entity { get; set; }
        public bool IsAlternative { get; set; }

        private bool IsComplex
        {
            get
            {
                return Request["DisplayMode"] == "1";
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            if (Entity == null)
                return;

            BindEntity(Entity, IsAlternative);
        }

        public void BindEntity(IIndexEntity entity, bool isAlternative)
        {
            if (isAlternative)
                infoLayout.Attributes["class"] = "alternative search-result";
            else
                infoLayout.Attributes["class"] = "search-result";

            lIntro.Text = entity.Subtitle;

            lMainInfo.Text = IsComplex ? entity.DisplayExtValues : entity.DisplayValues;

            if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                linkButtonName.Text = entity.Name;
            }
            else
            {
                linkButtonName.Text = string.Format("Item #{0}", entity.DynEntityUid);
                linkButtonName.Font.Italic = true;
            }

            linkButtonName.NavigateUrl =
                string.Format(Request.Url.GetLeftPart(UriPartial.Authority) +
                "/Asset/View.aspx?assetTypeUID={0}&assetUID={1}&SearchId={2}",
                    entity.DynEntityConfigUid, entity.DynEntityUid, Request["SearchId"]);
        }
    }
}