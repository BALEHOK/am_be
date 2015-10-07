using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Batch;
using AppFramework.Core.Classes.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.Taxonomies
{
    public partial class TreeEdit : BasePage
    {
        [Dependency]
        public IBatchJobFactory BatchJobFactory { get; set; }
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }

        private readonly ITaxonomyService _taxonomyService;

        public TreeEdit()
        {
            _taxonomyService = new TaxonomyService(UnitOfWork);
        }

        private long _taxonomyUid
        {
            get { return long.Parse(ViewState["TaxonomyUid"].ToString()); }
            set { ViewState["TaxonomyUid"] = value; }
        }

        private long TempItemUid
        {
            get
            {
                if (ViewState["TempItemUid"] == null) return default(long);
                return long.Parse(ViewState["TempItemUid"].ToString());
            }
            set { ViewState["TempItemUid"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            long uid = 0;
            if (!long.TryParse(Request.QueryString["Uid"], out uid))
                Response.Redirect("~/admin/Taxonomies/");

            CancelRenaming.Attributes.Add("onclick", "return hideRenamePanel()");

            if (IsPostBack)
            {
                var postBackerId = Request.Form.Get(Page.postEventSourceID);
                var postBackerArg = Request.Form.Get(Page.postEventArgumentID);
                if (postBackerId == "DeleteNode")
                {
                    ViewState["edited"] = true;
                    var node = TaxonomiesTree.FindNode(postBackerArg);
                    if (node.Parent != null)
                        node.Parent.ChildNodes.Remove(node);
                    else
                        TaxonomiesTree.Nodes.Remove(node);
                }

                Save.Visible = true;
                Publish.Visible = true;
            }
            else
            {
                var taxonomy = _taxonomyService.GetByUid(uid);

                // if edit enabled
                if (!string.IsNullOrEmpty(Request.QueryString["Edit"]))
                {
                    var latest = _taxonomyService.GetLastById(taxonomy.ID);
                    if (latest != null && latest.IsDraft)
                    {
                        if (latest.UID != taxonomy.UID)
                        {
                            Response.Redirect(string.Format("~/admin/Taxonomies/TreeEdit.aspx?Uid={0}&Edit=1",
                                                            latest.UID));
                        }
                        DraftAlert.Visible = true;
                        Publish.Visible = true;
                    }
                }
                else
                {
                    TaxTextRequiredFieldValidator.Visible = false;
                    linkAddNode.Visible = false;
                }

                BuildTree(taxonomy);
                this.TempItemUid = 0;
            }
        }

        private void BuildTree(Taxonomy taxonomy)
        {
            _taxonomyUid = taxonomy.UID;
            TaxonomiesTree.Nodes.Clear();
            foreach (TaxonomyItem item in taxonomy.RootItems)
                TaxonomiesTree.Nodes.Add(TaxonomyToNode(item));
            TaxonomiesTree.ExpandAll();
        }

        private TreeNode TaxonomyToNode(TaxonomyItem item)
        {
            TreeNode tn = new TreeNode()
            {
                Text = item.Name,
                Value = item.Uid.ToString(),
                SelectAction = TreeNodeSelectAction.None
            };
            foreach (TaxonomyItem ch in item.ChildItems)
                tn.ChildNodes.Add(TaxonomyToNode(ch));
            return tn;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Edit"]))
            {
                foreach (TreeNode node in TaxonomiesTree.Nodes)
                {
                    AddExtraLinkToNode(node);
                }
            }

            TaxonomiesTree.ExpandAll();
            base.OnPreRender(e);
        }

        private void AddExtraLinkToNode(TreeNode node)
        {
            string originalText = Server.HtmlEncode(GetNodeText(node)); // node.Text.Replace("'", @"\'");           

            // add Rename command
            node.Text = string.Format("{3}&nbsp;&nbsp;<a href='#' onclick=\"ShowRenamePane('{0}', '{1}')\">{2}</a>&nbsp;/",
                node.ValuePath, originalText, this.GetLocalResourceObject("Rename"), originalText);

            if (!node.Text.Contains("AddNode("))
            {
                node.Text += string.Format("&nbsp;<a href='#' onclick=\"return AddNode('{0}','{1}')\">{2}</a>",
                        node.ValuePath, originalText, Resources.Global.AddNodeText);
            }

            if (!node.Text.Contains("delbutton"))
            {
                node.Text +=
                        string.Format("  <a href='javascript:void(0);' class='delbutton' onclick='DeleteNode(\"{0}\")'>"
                                    + "{1}</a>", node.ValuePath, Resources.Global.DeleteNodeText);
            }

            // deny to manage types for modified tree
            if (ViewState["edited"] == null || (bool)ViewState["edited"] == false)
            {
                if (!node.Text.Contains("/ManageTypes"))
                    node.Text += string.Format(" / <a href='{1}?Uid={0}'>{2}</a>",
                        node.Value, (new System.Web.UI.Control()).ResolveUrl("~/admin/Taxonomies/ManageTypes.aspx"),
                        Resources.Global.ManageTypesText);
            }
            else
            {
                Regex re = new Regex(@"\s/\s");
                // node.Text = re.Split(node.Text)[0];
            }
            node.Text += string.Format(@"&nbsp;&nbsp;&nbsp;<a href=""javascript:void(0)"" onclick=""DownTaxonomyItem('{0}')""><img src=""/images/arrowdown.png""/></a>&nbsp;
                                         <a href=""javascript:void(0)"" onclick=""UpTaxonomyItem('{0}')""><img src=""/images/arrowup.png""/></a>", node.ValuePath);
            foreach (TreeNode ch in node.ChildNodes)
            {
                AddExtraLinkToNode(ch);
            }
        }

        protected void RenameTaxonomy(object sender, EventArgs e)
        {
            if (NodePath.Value != string.Empty)
            {
                TreeNode node = TaxonomiesTree.FindNode(NodePath.Value);
                if (node != null)
                {
                    node.Text = NewTaxonomyName.Text;
                }
            }

            ViewState["edited"] = true;
        }

        /// <summary>
        /// Adds the taxonomy to current selected node.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddTaxonomyToCurrent(object sender, EventArgs e)
        {
            if (NodePath.Value != string.Empty)
            {
                TreeNode node = TaxonomiesTree.FindNode(NodePath.Value);
                if (node != null)
                {
                    node.ChildNodes.Add(GetNewNode());
                }
            }
            else
            {
                TaxonomiesTree.Nodes.Add(GetNewNode());
            }
            TaxText.Text = string.Empty;
            ViewState["edited"] = true;
        }


        protected void btnDownTaxonomyItem_Click(object sender, EventArgs e)
        {
            TreeNode node = TaxonomiesTree.FindNode(hfChangeTaxonomyItemLocation.Value);
            if (node != null)
            {
                TreeNode parentNode = node.Parent;
                if (parentNode != null)
                {
                    int index = parentNode.ChildNodes.IndexOf(node);
                    if (index + 1 < parentNode.ChildNodes.Count)
                    {
                        parentNode.ChildNodes.RemoveAt(index);
                        parentNode.ChildNodes.AddAt(index + 1, node);
                        ViewState["edited"] = true;
                    }
                }
                else
                {
                    int index = TaxonomiesTree.Nodes.IndexOf(node);
                    if (index != -1 && index + 1 < TaxonomiesTree.Nodes.Count)
                    {
                        TaxonomiesTree.Nodes.RemoveAt(index);
                        TaxonomiesTree.Nodes.AddAt(index + 1, node);
                        ViewState["edited"] = true;
                    }
                }
            }
        }

        protected void btnUpTaxonomyItem_Click(object sender, EventArgs e)
        {
            TreeNode node = TaxonomiesTree.FindNode(hfChangeTaxonomyItemLocation.Value);
            if (node != null)
            {
                TreeNode parentNode = node.Parent;
                if (parentNode != null)
                {
                    int index = parentNode.ChildNodes.IndexOf(node);
                    if (index != 0)
                    {
                        parentNode.ChildNodes.RemoveAt(index);
                        parentNode.ChildNodes.AddAt(index - 1, node);
                        ViewState["edited"] = true;
                    }
                }
                else
                {
                    int index = TaxonomiesTree.Nodes.IndexOf(node);
                    if (index != -1 && index != 0)
                    {
                        TaxonomiesTree.Nodes.RemoveAt(index);
                        TaxonomiesTree.Nodes.AddAt(index - 1, node);
                        ViewState["edited"] = true;
                    }
                }
            }
        }

        private TreeNode GetNewNode()
        {
            this.TempItemUid -= 1; // all not saved items will have negative ID and will be processed by ProcessNode as new
            return new TreeNode()
            {
                Text = TaxText.Text.Trim(),
                Value = this.TempItemUid.ToString(),
                SelectAction = TreeNodeSelectAction.None
            };
        }

        /// <summary>
        /// Handles the Click event of the Save control. Save changes in taxonomie tree
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Save_Click(object sender, EventArgs e)
        {
            SaveChanges();
            Response.Redirect("~/admin/Taxonomies/");
        }

        private Taxonomy SaveChanges()
        {
            Taxonomy taxonomy = null;
            if (ViewState["edited"] != null && (bool)ViewState["edited"])
            {
                taxonomy = BuildTaxonomyFromTree();
                _taxonomyService.Save(taxonomy.Base, AuthenticationService.CurrentUserId);
                ViewState["edited"] = false;
            }
            return taxonomy;
        }

        /// <summary>
        /// Recursively synchonizes TreeView with Taxonomy tree
        /// </summary>
        /// <param name="nodes"></param>
        private Taxonomy BuildTaxonomyFromTree()
        {
            var taxonomy = _taxonomyService.GetByUid(_taxonomyUid);
            var items = new Dictionary<TreeNode, TaxonomyItem>();
            ProcessNodes(TaxonomiesTree.Nodes, taxonomy, items);
            taxonomy.Items.Clear();
            var taxonomyItems = (from pair in items
                                 where !pair.Value.ParentUid.HasValue
                                 select pair.Value).ToList();
            taxonomyItems.ForEach(i => taxonomy.Items.Add(i));
            return taxonomy;
        }

        private void ProcessNodes(TreeNodeCollection nodes, Taxonomy taxonomy, Dictionary<TreeNode, TaxonomyItem> items)
        {
            short index = 0;
            foreach (TreeNode node in nodes)
            {
                index++;
                long nodeUid = long.Parse(node.Value);
                TaxonomyItem taxonomyItem;
                if (nodeUid > 0)
                {
                    var existingItem = taxonomy.Items.Single(ti => ti.Uid == nodeUid);
                    existingItem.Name = GetNodeText(node);
                    existingItem.DisplayOrder = index;
                    // take existing entity
                    items.Add(node, existingItem);
                }
                else
                {
                    string nodeText = GetNodeText(node);

                    // build a new entity
                    taxonomyItem = new TaxonomyItem(
                        new AppFramework.Entities.TaxonomyItem
                        {
                            Name = nodeText,
                            Number = string.Empty,
                            ActiveVersion = true
                        });

                    if (node.Parent != null)
                    {
                        // add subnode to node
                        items[node.Parent].ChildItems.Add(taxonomyItem);
                    }
                    // add node 
                    taxonomyItem.DisplayOrder = index;
                    items.Add(node, taxonomyItem);
                }
                ProcessNodes(node.ChildNodes, taxonomy, items); // move recursively down 
            }
        }

        private string GetNodeText(TreeNode node)
        {
            Regex re = new Regex(@"\<a");
            string nodeText = re.Split(Server.HtmlDecode(node.Text))[0].Trim(); // get node text without add/manage links text TODO: avoid using re and node value as text
            return nodeText;
        }

        /// <summary>
        /// Handles the Click event of the Publish control. Save changes and make taxonomie active
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Publish_Click(object sender, EventArgs e)
        {
            SaveChanges();
            var job = BatchJobFactory.CreateTaxonomyJob(_taxonomyUid, AuthenticationService.CurrentUserId);
            BatchJobManager.SaveJob(job);
            Cache<Taxonomy>.Flush();
            Response.Redirect(job.NavigateUrl);
        }

        /// <summary>
        /// Cancels the editing/viewing with no save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/admin/Taxonomies/");
        }
    }
}