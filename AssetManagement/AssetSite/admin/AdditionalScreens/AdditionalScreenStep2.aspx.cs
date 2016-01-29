using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Entities;
using AppFramework.Core.Classes.ScreensServices;

namespace AssetSite.admin.AdditionalScreens
{
    public partial class AdditionalScreenStep2 : ScreensController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (IsPostBack)
                return;
            chkIsDeafult.Checked = _currentScreen.IsDefault;
            chkIsMobile.Checked = _currentScreen.IsMobile;
            txtName.Text = _currentScreen.Name;
            tbTitle.Text = _currentScreen.Title;
            tbSubTitle.Text = _currentScreen.Subtitle;
            tbPageText.Text = _currentScreen.PageText;
            tbDesc.Text = _currentScreen.Comment;

            var linkedAssetTypeAttributesUids = _currentScreen
                .DynEntityAttribScreens
                .Select(a => a.DynEntityAttribUid)
                .ToList();

            var linkedAssets =
                   (from a in _currentType.Attributes
                    where a.IsActive && a.IsShownOnPanel && (a.IsAsset || a.IsMultipleAssets) && !a.IsUpdateUser
                    select new SelectedAssetDescriptor
                    {
                        Attribute = a,
                        IsChecked = false
                    }).ToList();

            if (linkedAssetTypeAttributesUids.Any())
            {
                for (int i = 0; i < linkedAssets.Count; i++)
                {
                    if (linkedAssetTypeAttributesUids.Any(uid => uid == linkedAssets[i].Attribute.UID))
                    {
                        linkedAssets[i].IsChecked = true;
                    }
                }
            }

            linkedItemsPanel.Visible = linkedAssets.Any();
            repLinkedAssets.DataSource = linkedAssets;
            repLinkedAssets.DataBind();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentScreen != null && _currentType != null)
            {
                _currentScreen.DynEntityAttribScreens.Clear();

                var removedRelatedAssetAttributes = new List<long>();

                foreach (RepeaterItem item in repLinkedAssets.Items)
                {
                    var linkedAssetCheckBox = (CheckBox)item.FindControl("cbLinkedAsset");
                    var hf = (HiddenField)item.FindControl("hdfAssetId");

                    var attribute = _currentType.Attributes.Single(a => a.UID == long.Parse(hf.Value));

                    if (linkedAssetCheckBox.Checked)
                    {
                        _currentScreen.DynEntityAttribScreens.Add(new DynEntityAttribScreens
                        {
                            DynEntityAttribUid = attribute.UID,
                            ScreenId = _currentScreen.ScreenId
                        });
                    }
                }

                var panels = UnitOfWork.AttributePanelRepository
                    .Get(g => g.ScreenId == _currentScreen.ScreenId,
                        include: entity => entity.AttributePanelAttribute.Select(a => a.DynEntityAttribConfig))
                    .OrderBy(p => p.DisplayOrder)
                    .ThenBy(p => p.AttributePanelUid);

                var apaToRemove =
                    panels.SelectMany(p => p.AttributePanelAttribute)
                        .Where(
                            a =>
                                a.ReferencingDynEntityAttribConfigId != null &&
                                removedRelatedAssetAttributes.Contains((long)a.ReferencingDynEntityAttribConfigId))
                        .ToList();

                UnitOfWork.AttributePanelAttributeRepository.Delete(apaToRemove);

                _currentScreen.Comment = tbDesc.Text;
                _currentScreen.Name = txtName.Text;
                _currentScreen.PageText = tbPageText.Text;
                _currentScreen.Status = Convert.ToInt32(ddlStatus.SelectedValue);
                _currentScreen.Subtitle = tbSubTitle.Text;
                _currentScreen.Title = tbTitle.Text;
                _currentScreen.IsDefault = chkIsDeafult.Checked;
                _currentScreen.IsMobile = chkIsMobile.Checked;
                _currentScreen.UpdateDate = DateTime.Now;
                _currentScreen.UpdateUserId = AuthenticationService.CurrentUserId;
                _currentScreen.DynEntityConfigUid = _currentType.UID;
                _currentScreen.LayoutId = LayoutRepository
                    .GetByType(LayoutType.List)
                    .Id;

                ScreensService.Save(_currentScreen);

                Response.Redirect(string.Format("AdditionalScreenStep3.aspx?ScreenId={0}&atuid={1}",
                    _currentScreen.ScreenId, _currentType.UID));
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("AdditionalScreenStep1.aspx?ScreenId={0}&atuid={1}",
                    _currentScreen.ScreenId, _currentType.UID));
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            if (!TryRedirectToBatch())
                Response.Redirect("~/Wizard/EditAssetType.aspx");
        }
    }
}