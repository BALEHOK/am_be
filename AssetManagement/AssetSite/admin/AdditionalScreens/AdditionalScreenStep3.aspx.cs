using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class AdditionalScreenStep3 : ScreensController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack)
            {
                var layouts = LayoutRepository.GetAll().ToList();

                LayoutRadioSelect.DataSource = layouts.Select(l => new
                {
                    ID = l.Id,
                    ImageName = string.Format("/images/layouts/layout{0}.png", (int)l.Type)
                }).ToList();
                LayoutRadioSelect.DataTextField = "ImageName";
                LayoutRadioSelect.DataValueField = "ID";
                LayoutRadioSelect.DataBind();
            }
        }

        protected void layoutListBound(object sender, EventArgs e)
        {
            if (!IsPostBack && _currentScreen.LayoutId != 0)
            {
                foreach (ListItem item in LayoutRadioSelect.Items)
                {
                    if (item.Value == _currentScreen.LayoutId.ToString())
                    {
                        item.Selected = true;
                    }
                }
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {      
            _currentScreen.LayoutId = Convert.ToInt32(LayoutRadioSelect.SelectedValue);
            ScreensService.Save(_currentScreen);
            Response.Redirect(string.Format("AdditionalScreenStep4.aspx?ScreenId={0}&atuid={1}",
                _currentScreen.ScreenId, _currentType.UID));
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("AdditionalScreenStep2.aspx?ScreenId={0}&atuid={1}",
                _currentScreen.ScreenId, _currentType.UID));
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Session["currentScreen"] = null;
            if (!TryRedirectToBatch())
                Response.Redirect("~/Wizard/EditAssetType.aspx");
        }
    }
}