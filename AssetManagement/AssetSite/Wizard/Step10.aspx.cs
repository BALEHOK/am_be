using System.Collections;
using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Wizard
{
    public partial class Step10 : WizardController
    {
        [Dependency]
        public ITaxonomyService TaxonomyService { get; set; }
        [Dependency]
        public ITaxonomyItemService TaxonomyItemService { get; set; }
        [Dependency]
        public IAssetTypeTaxonomyManager AssetTypeTaxonomyManager { get; set; }

        private Taxonomy _taxonomy;
        public static string TREE_IDENT_CONST = "TaxonomiesAll_TREE_";

        protected override void btnClose_Click(object sender, EventArgs e)
        {
            base.btnClose_Click(sender, e);
        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            AddCurrentTaxonomyToChanged();
            Session.Add(SessionVariables.AssetTypeWizard_CurrentTaxonomy, _taxonomy);
            Response.Redirect("~/Wizard/Step11.aspx");
        }

        protected override void btnPrevious_Click(object sender, EventArgs e)
        {
            AddCurrentTaxonomyToChanged();
            Session.Add(SessionVariables.AssetTypeWizard_CurrentTaxonomy, _taxonomy);
            Response.Redirect("~/Wizard/Step9.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetType == null)
                Response.Redirect("~/Wizard/Step1.aspx");

            if (Session[SessionVariables.AssetTypeWizard_CurrentTaxonomy] != null)                   // selected taxonomy
            {
                _taxonomy = Session[SessionVariables.AssetTypeWizard_CurrentTaxonomy] as Taxonomy;
            }
            else
            {
                _taxonomy = TaxonomyService.GetCategory();
            }

            if (!IsPostBack && _taxonomy != null)
            {
                if (Session["AssetTypeWizardID"] != null)
                {
                    var id = (long)Session["AssetTypeWizardID"];
                    if (id == AssetType.ID)
                    {
                        if (Session[TREE_IDENT_CONST + _taxonomy.Name] == null)
                        {
                            PopulateRootLevel();
                            var nodes = TreeView1.Nodes.Cast<TreeNode>().ToList();
                            Session[TREE_IDENT_CONST + _taxonomy.Name] = nodes;
                        }
                        else
                        {
                            TreeView1.Nodes.Clear();
                            var nodes2 = (List<TreeNode>)Session[TREE_IDENT_CONST + _taxonomy.Name];
                            foreach (var treeItem in nodes2)
                            {
                                TreeView1.Nodes.Add(treeItem);
                            }
                        }
                    }
                    else
                    {
                        PopulateRootLevel();
                    }
                }
                else
                {
                    Session["AssetTypeWizardID"] = AssetType.ID;
                    PopulateRootLevel();
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            BindTaxonomyList();
        }

        private void BindTaxonomyList()
        {
            var result = TaxonomiesAll.Items.FindByText(_taxonomy.Name);
            TaxonomiesAll.SelectedValue = result.Value;
        }

        private void PopulateRootLevel()
        {
            TreeView1.Nodes.Clear();
            PopulateNodes(_taxonomy.RootItems.ToList(), TreeView1.Nodes);
        }

        private void PopulateNodes(IEnumerable<TaxonomyItem> taxonomyItems, TreeNodeCollection nodes)
        {
            // lambda function to set node as checked/unchecked
            Func<TaxonomyItem, bool> isChecked = item =>
                TaxonomyItemService
                .GetAssignedAssetTypes(item.Base)
                .Any(at => at.ID == AssetType.ID);

            if (AssetType.IsUnpublished && Session[SessionVariables.AssetTypeWizard_Taxonomies] == null)
            {
                var containers = AssetTypeTaxonomyManager.GetContainers(AssetType.ID);
                Session[SessionVariables.AssetTypeWizard_Taxonomies] = containers;
                var currentContainer = containers.SingleOrDefault(c => c.TaxonomyUid == _taxonomy.UID);

                if (currentContainer != null)
                    isChecked = item => currentContainer.AssignedTaxonomyItemsIds.Contains(item.Id);
            }

            foreach (var item in taxonomyItems)
            {
                var tn = new TreeNode {Text = item.Name, Value = item.Uid.ToString(), Checked = isChecked(item)};
                nodes.Add(tn);
                PopulateNodes(item.ChildItems.ToList(), tn.ChildNodes);
            }
        }

        private void PopulateSubLevel(long parentUid, TreeNode parentNode)
        {
            var items = _taxonomy.Items.Where(ti => ti.ParentUid == parentUid).ToList();
            if (items.Count > 0)
            {
                PopulateNodes(items, parentNode.ChildNodes);
            }
        }

        protected void TreeView1_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            PopulateSubLevel(long.Parse(e.Node.Value), e.Node);
        }
        
        protected void TreeView1_TreeNodeCheckChanged(Object sender, TreeNodeEventArgs e)
        {
            var nodes = TreeView1.Nodes.Cast<TreeNode>().ToList();
            Session[TREE_IDENT_CONST + _taxonomy.Name] = nodes;
        }


        protected void SelectedTaxonomyChanged(Taxonomy tax)
        {
            AddCurrentTaxonomyToChanged();
            Session[SessionVariables.AssetTypeWizard_CurrentTaxonomy] = tax;
            _taxonomy = tax;
            if (Session[TREE_IDENT_CONST + _taxonomy.Name] != null)
            {
                TreeView1.Nodes.Clear();
                var nodes2 = (List<TreeNode>)Session[TREE_IDENT_CONST + _taxonomy.Name];
                foreach (var treeItem in nodes2)
                {
                    TreeView1.Nodes.Add(treeItem);
                }
            }
            else
            {
                PopulateRootLevel();
                var nodes = TreeView1.Nodes.Cast<TreeNode>().ToList();
                Session[TREE_IDENT_CONST + _taxonomy.Name] = nodes;
            }
        }

        private void AddCurrentTaxonomyToChanged()
        {
            if (Session[SessionVariables.AssetTypeWizard_Taxonomies] as List<TaxonomyContainer> == null)
                Session[SessionVariables.AssetTypeWizard_Taxonomies] = new List<TaxonomyContainer>();
            var containers = Session[SessionVariables.AssetTypeWizard_Taxonomies] as List<TaxonomyContainer>;
            var container = containers.SingleOrDefault(c => c.TaxonomyUid == _taxonomy.UID); 
            if (container == null)
            {
                container = new TaxonomyContainer { TaxonomyUid = _taxonomy.UID };
                containers.Add(container);
            }
            container.AssignedTaxonomyItemsIds = SelectCheckedNodesIds(TreeView1.Nodes).ToList();
            Session[SessionVariables.AssetTypeWizard_Taxonomies] = containers;
        }

        private IEnumerable<long> SelectCheckedNodesIds(IEnumerable nodeCollection)
        {
            foreach (TreeNode node in nodeCollection)
            {
                if (node.Checked)
                {
                    var item = _taxonomy.Items.Single(i => i.Uid == long.Parse(node.Value));
                    yield return item.Id;
                }
                foreach (var x in SelectCheckedNodesIds(node.ChildNodes))
                {
                    yield return x;
                }
            }
        }
    }
}
