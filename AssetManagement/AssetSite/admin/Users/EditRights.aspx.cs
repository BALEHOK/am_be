using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;
using AppFramework.Core.Services;

namespace AssetSite.admin.Users
{
    public partial class AddRights : BasePage
    {
        [Dependency]
        public ITaxonomyService TaxonomyService { get; set; }
        [Dependency]
        public ITaxonomyItemService TaxonomyItemService { get; set; }
        [Dependency]
        public IRightsService RightsService { get; set; }

        protected string Username { get; set; }

        private long _userId;
        private long _viewId;
        private List<RightsEntry> _rights;
        private TaskRightsList _taskrights;
        private bool _istask;

        protected void Page_Load(object sender, EventArgs e)
        {
            _istask = !string.IsNullOrEmpty(Request.QueryString["type"]) && Request.QueryString["type"].ToLower() == "task";
            long.TryParse(Request.QueryString["userid"], out _userId);
            long.TryParse(Request.QueryString["viewid"], out _viewId);

            if (_userId == 0 || (_viewId == 0 && _userId == 0))
                Response.Redirect("~/admin/Users/ViewRights.aspx");

            if (_viewId > 0)
            {
                if (_istask)
                {
                    _taskrights = TaskRightsList.GetByViewID(_viewId);
                }
                else
                {
                    _rights = GetRightsByViewId(_viewId);
                }
            }

            atGrid.DataSource = AssetTypeRepository.GetAllPublished().OrderBy(a => a.Name);
            atGrid.DataBind();

            var assetType = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Department);
            deptGrid.DataSource
                = AssetsService.GetAssetsByAssetTypeAndUser(assetType, AuthenticationService.CurrentUserId);
            deptGrid.DataBind();

            if (!IsPostBack)
            {
                #region Setting the Taxonomy tree data
                taxonomyTree.Taxonomy = taxonomiesDropDown.DefaultTaxonomy;
                taxonomyTree.BuildTree();
                #endregion

                if (_istask)
                {
                    lblPermissions.Visible = permissionsSet.Visible = false;
                    ruleConditions.Items.RemoveAt(ruleConditions.Items.Count - 1);
                }
                
                if (_viewId > 0)
                {
                    if (_istask)
                    {
                        var item = _taskrights.Items.FirstOrDefault(it => it.TaxonomyItemId != null && it.TaxonomyItemId > 0);
                        if (item != null)
                        {
                            var taxonomyItem = TaxonomyItemService.GetActiveItemById(item.TaxonomyItemId.Value);
                            if (taxonomyItem != null)
                            {
                                taxonomiesDropDown.SelectedValue = taxonomyItem.TaxonomyUid.ToString();
                                taxonomiesDropDown_Changed(TaxonomyService.GetByUid(taxonomyItem.TaxonomyUid));
                            }
                        }
                        ruleType.SelectedIndex = (_taskrights.Items.First().IsDeny) ? 1 : 0;
                        categoriesPanel.Visible = ruleConditions.Items[0].Selected
                            = _taskrights.Items.Any(r => r.TaxonomyItemId > 0);
                        assetTypesPanel.Visible = ruleConditions.Items[1].Selected
                            = _taskrights.Items.Any(r => r.DynEntityConfigId > 0);
                        if (_taskrights.Items.Any(i => i.DynEntityConfigId > 0) ||
                            _taskrights.Items.Any(i => i.TaxonomyItemId > 0))
                        {
                            foreach (TaskRightsEntry entry in _taskrights.Items)
                            {
                                SetCheckedNodes(entry.TaxonomyItemId, taxonomyTree.Nodes);
                            }
                            foreach (GridViewRow row in atGrid.Rows)
                            {
                                CheckBox chk = row.Cells[1].FindControl("atCheckbox") as CheckBox;
                                HiddenField idField = row.Cells[1].FindControl("atID") as HiddenField;
                                chk.Checked = (_taskrights.Items.Any(r => r.DynEntityConfigId == long.Parse(idField.Value)));
                            }
                        }
                        else
                        {
                            propagateGroup.SelectedIndex = 0;
                            ruleConditions.Visible = false;
                        }
                    }
                    else
                    {
                        var item = _rights.FirstOrDefault(it => it.TaxonomyItemId > 0);
                        if (item != null)
                        {
                            var taxonomyItem = TaxonomyItemService.GetActiveItemById(item.TaxonomyItemId);
                            if (taxonomyItem != null)
                            {
                                taxonomiesDropDown.SelectedValue = taxonomyItem.TaxonomyUid.ToString();
                                taxonomiesDropDown_Changed(TaxonomyService.GetByUid(taxonomyItem.TaxonomyUid));
                            }
                        }

                        ruleType.SelectedIndex = (_rights.First().IsDeny) ? 1 : 0;
                        categoriesPanel.Visible = ruleConditions.Items[0].Selected
                            = _rights.Any(r => r.TaxonomyItemId > 0);
                        assetTypesPanel.Visible = ruleConditions.Items[1].Selected
                            = _rights.Any(r => r.AssetTypeID > 0);
                        departmentsPanel.Visible = ruleConditions.Items[2].Selected
                            = _rights.Any(r => r.DepartmentID > 0);


                        if (_rights.Any(i => i.AssetTypeID > 0) ||
                       _rights.Any(i => i.TaxonomyItemId > 0) ||
                       _rights.Any(i => i.DepartmentID > 0))
                        {
                            foreach (RightsEntry entry in _rights)
                            {
                                SetCheckedNodes(entry.TaxonomyItemId, taxonomyTree.Nodes);
                            }
                            foreach (GridViewRow row in atGrid.Rows)
                            {
                                CheckBox chk = row.Cells[1].FindControl("atCheckbox") as CheckBox;
                                HiddenField idField = row.Cells[1].FindControl("atID") as HiddenField;
                                chk.Checked = (_rights.Any(r => r.AssetTypeID == long.Parse(idField.Value)));
                            }
                            foreach (GridViewRow row in deptGrid.Rows)
                            {
                                CheckBox chk = row.Cells[1].FindControl("deptCheckbox") as CheckBox;
                                HiddenField idField = row.Cells[1].FindControl("deptID") as HiddenField;
                                chk.Checked = (_rights.Any(r => r.DepartmentID == long.Parse(idField.Value)));
                            }
                        }
                        else
                        {
                            propagateGroup.SelectedIndex = 0;
                            ruleConditions.Visible = false;
                        }
                        var permission = _rights.First().Permission;
                        permissionsSet.Items[0].Selected = permission.CanDelete() || permission.CanRead();
                        permissionsSet.Items[1].Selected = permission.CanDelete() || permission.CanWrite();
                        permissionsSet.Items[2].Selected = permission.CanDelete() || permission.CanRead(true);
                        permissionsSet.Items[3].Selected = permission.CanDelete() || permission.CanWrite(true);
                        permissionsSet.Items[4].Selected = permission.CanDelete();
                    }
                }
                else
                {
                    taxonomiesDropDown.SelectedValue = taxonomiesDropDown.DefaultTaxonomy.UID.ToString();
                }
            }
            Username = Membership.GetUser(_userId, false).UserName;
        }

        private void SetCheckedNodes(long? categoryId, TreeNodeCollection nodes)
        {
            if (categoryId == null) return;
            foreach (TreeNode node in nodes)
            {
                if (long.Parse(node.Value) == categoryId)
                {
                    node.Checked = true;
                }
                if (node.ChildNodes.Count > 0)
                {
                    SetCheckedNodes(categoryId, node.ChildNodes);
                }
            }
        }

        protected void taxonomiesDropDown_Changed(Taxonomy tax)
        {
            taxonomyTree.Taxonomy = tax;
            taxonomyTree.BuildTree();
            if (_istask)
            {
                if (_taskrights != null)
                {
                    foreach (TaskRightsEntry entry in _taskrights.Items)
                    {
                        SetCheckedNodes(entry.TaxonomyItemId, taxonomyTree.Nodes);
                    }
                }
            }
            else
            {
                if (_rights != null)
                {
                    foreach (RightsEntry entry in _rights)
                    {
                        SetCheckedNodes(entry.TaxonomyItemId, taxonomyTree.Nodes);
                    }
                }
            }
        }

        private IEnumerable<long> GetCheckedValues(TreeNodeCollection nodes)
        {
            List<long> returnList = new List<long>();
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                {
                    returnList.Add(long.Parse(node.Value));
                }
                if (node.ChildNodes.Count > 0)
                {
                    returnList.AddRange(GetCheckedValues(node.ChildNodes));
                }
            }
            return returnList;
        }

        protected void btnSave_Click(Object sender, EventArgs e)
        {

            List<long> assetTypes = new List<long>();
            List<long> departments = new List<long>();
            List<long> taxonomyItems = new List<long>();

            // rule behavior
            bool isDeny = (ruleType.SelectedValue == "0") ? false : true;

            // proparate on specific items is enabled
            if (propagateGroup.Items.FindByValue("1").Selected)
            {
                // get checked taxonomy items
                if (categoriesPanel.Visible)
                {
                    taxonomyItems = new List<long>(GetCheckedValues(taxonomyTree.Nodes));
                }

                // get checked asset types
                if (assetTypesPanel.Visible)
                {
                    foreach (GridViewRow row in atGrid.Rows)
                    {
                        CheckBox chk = row.Cells[1].FindControl("atCheckbox") as CheckBox;
                        HiddenField idField = row.Cells[1].FindControl("atID") as HiddenField;
                        if (chk != null && idField != null && chk.Checked)
                        {
                            assetTypes.Add(long.Parse(idField.Value));
                        }
                    }
                }

                // get checked departments
                if (departmentsPanel.Visible)
                {
                    foreach (GridViewRow row in deptGrid.Rows)
                    {
                        CheckBox chk = row.Cells[1].FindControl("deptCheckbox") as CheckBox;
                        HiddenField idField = row.Cells[1].FindControl("deptID") as HiddenField;
                        if (chk != null && idField != null && chk.Checked)
                        {
                            departments.Add(long.Parse(idField.Value));
                        }
                    }
                }
            }

            //// convert checkboxes state (binary code like 1010) to dec number
            //double pCode = 0;
            //int count = permissionsSet.Items.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    // а n-1 х 2^n-1 + ... + а 1 х 2^1 + а 0 х 2^0
            //    pCode += (permissionsSet.Items[i].Selected ? 1 : 0) * Math.Pow(2, (count - 1) - i);
            //}
            //Permission permission = PermissionsProvider.GetByCode(byte.Parse(pCode.ToString()));

            // get the permissions
            Permission permission;
            if (!permissionsSet.Items[4].Selected)
            {
                permission = PermissionsProvider.Or(
                    permissionsSet.Items[0].Selected ? Permission.RDDD : Permission.DDDD,
                    permissionsSet.Items[1].Selected ? Permission.DWDD : Permission.DDDD,
                    permissionsSet.Items[2].Selected ? Permission.DDRD : Permission.DDDD,
                    permissionsSet.Items[3].Selected ? Permission.DDDW : Permission.DDDD);
            }
            else
            {
                permission = Permission.ReadWriteDelete;
            }

            // make the rights list
            var rightsList = new List<RightsEntry>();
            TaskRightsList taskrightsList = new TaskRightsList();

            // if any of lists is empty, add zero value to be sure,
            // that loop will be executed once.
            // JW's great idea :)
            if (taxonomyItems.Count == 0) taxonomyItems.Add(default(long));
            if (assetTypes.Count == 0) assetTypes.Add(default(long));
            if (departments.Count == 0) departments.Add(default(long));

            // filling the list of rules
            if (_istask)
            {
                foreach (long taxID in taxonomyItems)
                {
                    foreach (long atID in assetTypes)
                    {
                        taskrightsList.Items.Add(
                                   new TaskRightsEntry()
                                   {
                                       TaxonomyItemId = taxID,
                                       DynEntityConfigId = atID,
                                       IsDeny = isDeny,
                                       ViewID = _viewId
                                   });
                    }
                }

                TaskRightsList.SetToUserByUserID(taskrightsList, _userId, _viewId);
            }
            else
            {
                foreach (long taxID in taxonomyItems)
                {
                    foreach (long atID in assetTypes)
                    {
                        foreach (long deptID in departments)
                        {
                            rightsList.Add(
                                new RightsEntry()
                                {
                                    TaxonomyItemId = taxID,
                                    AssetTypeID = atID,
                                    DepartmentID = deptID,
                                    Permission = permission,
                                    IsDeny = isDeny,
                                    ViewID = _viewId
                                });
                        }
                    }
                }

                // assign rights to user
                RightsService.SetPermissionsForUser(
                    rightsList, 
                    _userId, 
                    AuthenticationService.CurrentUserId,
                    _viewId);

                // force rights updating for edited user
                AccessManager.Instance.ForceRightsUpdate(_userId);
            }
            Response.Redirect("~/admin/Users/ViewRights.aspx");
        }

        protected void btnCancel_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/Users/ViewRights.aspx");
        }

        protected void ruleConditions_Changed(Object sender, EventArgs e)
        {
            foreach (ListItem item in ruleConditions.Items)
            {
                switch (item.Value)
                {
                    case "0":
                        categoriesPanel.Visible = item.Selected;
                        break;

                    case "1":
                        assetTypesPanel.Visible = item.Selected;
                        break;

                    case "2":
                        departmentsPanel.Visible = item.Selected;
                        break;
                }
            }
        }

        protected void propagateGroup_Changed(object sender, EventArgs e)
        {
            ruleItems.Visible = ruleConditions.Visible = !propagateGroup.Items.FindByValue("0").Selected;
        }

        /// <summary>
        /// Returns the list of permissions 
        /// by given ViewID
        /// </summary>
        /// <param name="viewID"></param>
        private List<RightsEntry> GetRightsByViewId(long viewID)
        {
            return (from entry in UnitOfWork.RightsRepository.Get(r => r.ViewId == viewID)
                    select new RightsEntry(entry)).ToList();
        }
    }
}
