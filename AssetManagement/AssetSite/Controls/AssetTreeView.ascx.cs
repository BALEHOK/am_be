using System.Web.UI;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AssetSite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;
using AssetManager.Infrastructure.Services;

namespace AssetSite.Controls
{
    public partial class AssetTreeView : UserControl
    {
        [Dependency]
        public IUserService UserService { get; set; }
        [Dependency]
        public AppFramework.Core.Classes.ITaxonomyService TaxonomyService { get; set; }
        [Dependency]
        public ITaxonomyItemService TaxonomyItemService { get; set; }
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }
        [Dependency]
        public ITasksService TasksService { get; set; }

        public delegate void AssetTypeSelectedHandler(AssetType assetType, TaxonomyItem item);
        public event AssetTypeSelectedHandler AssetTypeSelectedEvent;
        private TaskRightsList _userTaskRightList;

        public TreeNode SelectedNode
        {
            get
            {
                return AssetTree.SelectedNode;
            }
        }

        public bool IsTask { get; set; }

        public long SelectedAssetTypeId
        {
            get
            {
                long id = 0;
                if (AssetTree.SelectedNode != null)
                {
                    string[] nodeId = AssetTree.SelectedNode.Value.Split(new char[] { '$' });
                    if (nodeId.Count() == 2 && nodeId[0] == "at")
                    {
                        long.TryParse(nodeId[1], out id);
                    }
                }
                return id;
            }
            set
            {
                foreach (TreeNode node in AssetTree.Nodes)
                {
                    SelectNodeByAssetTypeId(value, node);
                }
            }
        }

        private Taxonomy Category
        {
            get
            {
                if (_category == null)
                {
                    _category = TaxonomyService.GetCategory();
                }
                return _category;
            }
        }

        private Taxonomy _category;
        private bool _isEventRaised = false;
     
        protected override void OnLoad(EventArgs e)
        {
            base.OnInit(e);
       
            if (!IsPostBack)
            {
                if (IsTask)
                {
                    _userTaskRightList = UserService.GetUserTaskRightsList(AuthenticationService.CurrentUserId);
                }
                PopulateRootLevel();
                RestoreLastClickedNode();
            }
        }

        private void RestoreLastClickedNode()
        {
            // restore last clicked node
            if (SessionWrapper.SelectedNodePath != null)
            {
                string[] nodeValues = SessionWrapper.SelectedNodePath.Split('/');

                // expanding hierarchically from root
                for (int i = 0; i < nodeValues.Count(); i++)
                {
                    // path from root to the current node
                    string path = String.Join("/", nodeValues, 0, i + 1);

                    // find node on this path
                    TreeNode n = AssetTree.FindNode(path);
                    if (n != null)
                    {
                        if (n.PopulateOnDemand)
                        {
                            PopulateSubLevel(n.Value, n);
                            n.PopulateOnDemand = false;
                            n.Expanded = true;
                        }
                        if (i == nodeValues.Count() - 1)
                        {
                            n.Selected = true;
                            if (n.Parent != null)
                            {
                                n.Parent.Expand();
                            }
                        }
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!IsPostBack)
            {
                RefreshState();
            }
        }

        private void PopulateRootLevel()
        {
            if (Category != null)
                PopulateNodes(Category.RootItems, AssetTree.Nodes);
        }

        private void SelectNodeByAssetTypeId(long atId, TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.ChildNodes)
            {
                if (node.Value.Contains("at$"))
                {
                    int index = node.Value.IndexOf("at$");
                    string val = node.Value.Substring(index, node.Value.Length - index);
                    string[] parts = val.Split(new char[1] { '$' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 && parts[0] == "at")
                    {
                        long tempID = 0;
                        if (long.TryParse(parts[1], out tempID))
                        {
                            if (tempID == atId)
                                node.Select();
                        }
                    }
                }
                else
                    this.SelectNodeByAssetTypeId(atId, node);
            }
        }

        private void PopulateNodes(IEnumerable<TaxonomyItem> dataItems, TreeNodeCollection nodes)
        {
            foreach (var item in dataItems)
            {
                if (IsTask && !AllowItemForUser(true, item.Id))
                    continue;
                var tn = new TreeNode();
                tn.Text = item.Name;
                tn.Value = "tax$" + item.Uid.ToString();
                tn.SelectAction = TreeNodeSelectAction.Expand;

                PopulateNodes(item.ChildItems, tn.ChildNodes);

                var assignedAssetTypes = TaxonomyItemService.GetAssignedAssetTypes(item.Base);
                foreach (var at in assignedAssetTypes.Where(at => !at.IsUnpublished))
                {
                    if (IsTask && !AllowItemForUser(false, at.ID))
                    {
                        continue;
                    }
                    if (!IsTask || TasksService.GetCountByAssetTypeId(at.ID) > 0)
                    {
                        var chAt = new TreeNode
                        {
                            Text = at.Name,
                            Value = "at$" + at.ID.ToString(),
                            SelectAction = TreeNodeSelectAction.Select,
                            PopulateOnDemand = false
                        };
                        tn.ChildNodes.Add(chAt);
                    }
                }
                nodes.Add(tn);
            }
        }

        private bool AllowItemForUser(bool isCategory, long id)
        {
            bool isallow = false;
            if (isCategory)
            {
                if (_userTaskRightList != null)
                {
                    if (_userTaskRightList.Items.Exists(delegate(TaskRightsEntry entry)
                    {
                        return entry.IsDeny &&
                            (entry.TaxonomyItemId == id || (entry.TaxonomyItemId == 0 && entry.DynEntityConfigId == 0));
                    }))
                    {
                        isallow = false;
                    }
                    else if (_userTaskRightList.Items.Exists(delegate(TaskRightsEntry entry)
                    {
                        return !entry.IsDeny &&
                            (entry.TaxonomyItemId == id || (entry.TaxonomyItemId == 0 && entry.DynEntityConfigId == 0));
                    }))
                    {
                        isallow = true;
                    }
                }
            }
            else
            {
                isallow = true;
                if (_userTaskRightList.Items.Exists(delegate(TaskRightsEntry entry)
                {
                    return entry.IsDeny &&
                        (entry.DynEntityConfigId == id);
                }))
                {
                    isallow = false;
                }
            }
            return isallow;
        }

        private void PopulateSubLevel(string parentUid, TreeNode parentNode)
        {
            string[] nodeUid = parentUid.Split(new char[] { '$' });
            if (nodeUid.Count() == 2 && nodeUid[0] == "tax")
            {
                List<TaxonomyItem> items
                    = Category.Items.Where(ti => ti.ParentUid == long.Parse(nodeUid[1])).ToList();
                if (items.Count > 0)
                {
                    PopulateNodes(items, parentNode.ChildNodes);
                }
            }
        }

        protected void AssetTree_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            PopulateSubLevel(e.Node.Value, e.Node);
        }

        protected void OnSelectedNodeChanged(object sender, EventArgs e)
        {
            RefreshState();
        }

        private void RefreshState()
        {
            if (!_isEventRaised)
            {
                TreeNode node = AssetTree.SelectedNode;

                if (node == null) return;

                string[] nodeUid = node.Value.Split(new char[] { '$' });
                // Current node is AT node
                if (nodeUid.Count() == 2 && nodeUid[0] == "at")
                {
                    TaxonomyItem ti = null;
                    string[] parentUid = node.Parent.Value.Split(new char[] { '$' });
                    if (parentUid.Count() == 2 && parentUid[0] == "tax")
                    {
                        ti = Category.Items.Single(t => t.Uid == long.Parse(parentUid[1]));
                    }
                    if (ti != null)
                    {
                        var assignedAssetTypes = TaxonomyItemService.GetAssignedAssetTypes(ti.Base);
                        var atype = assignedAssetTypes.SingleOrDefault(a => a.ID == long.Parse(nodeUid[1]));
                        if (atype != null && AssetTypeSelectedEvent != null)
                        {
                            AssetTypeSelectedEvent(atype, ti);
                            _isEventRaised = true;
                        }
                    }
                }
                AssetTree.SelectedNode.Selected = true;
                SessionWrapper.SelectedNodePath = node.ValuePath;
            }
        }

        protected void ExpandCollapseClick(object sender, EventArgs e)
        {
            if (Session["TreeExpanded"] != null && (bool)Session["TreeExpanded"] == true)
            {
                AssetTree.CollapseAll();
                Session["TreeExpanded"] = false;
            }
            else
            {
                AssetTree.ExpandAll();
                Session["TreeExpanded"] = true;
            }
        }
    }
}