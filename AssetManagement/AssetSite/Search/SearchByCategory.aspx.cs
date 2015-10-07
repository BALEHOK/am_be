using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;

namespace AssetSite.Search
{
    public partial class SearchByCategory : SearchPage
    {
        private string GUID
        {
            get
            {
                if (hfUid.Value == string.Empty)
                    hfUid.Value = Guid.NewGuid().ToString();
                return hfUid.Value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var taxonomies = UnitOfWork.TaxonomyRepository.Get(
                    t => t.ActiveVersion && t.IsActive,
                    orderBy: taxs => taxs.OrderByDescending(t => t.IsCategory).ThenBy(t => t.Name),
                    include: t => t.TaxonomyItem);

                List<TaxonomyTree> trees = (from t in taxonomies
                                            select new TaxonomyTree
                                            {
                                                Id = t.TaxonomyUid,
                                                Name = new AppFramework.Core.Classes.TranslatableString(t.Name).GetTranslation(),
                                                Tree = CreateTree(t.TaxonomyItem.OrderBy(tax => tax.DisplayOrder)),
                                                IsCategory = t.IsCategory
                                            }).ToList();

                Session["Tree" + GUID] = trees;
                ddlTaxonomy.DataSource = trees;
                ddlTaxonomy.DataTextField = "Name";
                ddlTaxonomy.DataValueField = "Id";
                ddlTaxonomy.DataBind();

                //var categoryTree = trees.SingleOrDefault(t => t.IsCategory);
                //if (categoryTree != null)
                //    ddlTaxonomy.SelectedIndex = trees.IndexOf(categoryTree);

                if (trees.Count > 0)
                {
                    UpdateTree(trees.First().Tree);
                }

                if (Request["TaxonomyItemsIds"] != null)
                {
                    _fillNodes(TreeView1.Nodes, Request["TaxonomyItemsIds"].ToString().Split(new char[] { ' ' }));
                }

                if (Request["Params"] != null)
                {
                    tbSearch.Text = Request["Params"].ToString();
                }

                if (Request["Time"] != null)
                {
                    TimePeriodForSearch period = (TimePeriodForSearch)int.Parse(Request.QueryString["Time"].ToString());
                    rbActive.SelectedIndex = period == TimePeriodForSearch.CurrentTime ? 0 : 1;
                }
            }
        }

        private void _fillNodes(TreeNodeCollection nodes, string[] taxids)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Value.Split(new char[] { ' ' }).Any(v => taxids.Contains(v)))
                {
                    node.Checked = true;
                }
                if (node.ChildNodes.Count > 0)
                {
                    _fillNodes(node.ChildNodes, taxids);
                }
            }
        }

        private void UpdateTree(List<XElement> tree)
        {
            TreeView1.Nodes.Clear();
            foreach (XElement element in tree)
            {
                TreeView1.Nodes.Add(element.GenerateTreeView());
            }

            if (TreeView1.Nodes.Count == 0)
            {
                EmptyTree.Text = Resources.Global.NoCategoriesMessage;
                EmptyTree.Visible = true;
                TreeView1.Visible = false;
            }
            else
            {
                EmptyTree.Visible = false;
                TreeView1.Visible = true;
            }
        }

        private void ParseNode(TreeNode node, out string ids, out string names)
        {
            ids = "";
            names = "";
            if (node.Checked)
            {
                ids = node.Value + " ";
                names = node.Text + " ";
            }
            string idsTmp;
            string namesTmp;
            foreach (TreeNode o in node.ChildNodes)
            {
                ParseNode(o, out idsTmp, out namesTmp);
                ids += idsTmp + " ";
                names += namesTmp + " ";
            }
            ids = ids.Trim();
            names = names.Trim();
            return;
        }

        private bool ParseTree(TreeNodeCollection nodes, out string ids, out string names)
        {
            ids = "";
            names = "";
            string idsTmp = "";
            string namesTmp = "";

            foreach (TreeNode node in nodes)
            {
                ParseNode(node, out idsTmp, out namesTmp);
                ids += idsTmp + " ";
                names += namesTmp + " ";
            }
            ids = ids.Trim();
            names = names.Trim();
            return ids != string.Empty;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string ids;
            string names;

            TimePeriodForSearch period = rbActive.SelectedValue == "1" ? TimePeriodForSearch.CurrentTime : TimePeriodForSearch.History;

            if (ParseTree(TreeView1.Nodes, out ids, out names))
            {
                var query = "?TaxItems=" +
                    Server.UrlEncode(names) +
                    "&TaxonomyItemsIds=" + Server.UrlEncode(ids) +
                    "&Params=" + Server.UrlEncode(tbSearch.Text.Trim()) +
                    "&Time=" + (int)period;

                Session[Constants.SearchParameters] = Request.Url.GetLeftPart(UriPartial.Path).Replace("ResultByCategory", "SearchByCategory") + query;

                Response.Redirect("~/Search/ResultByCategory.aspx" + query);
            }
        }

        protected void ddlTaxonomy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Session["Tree" + GUID] == null)
                return;
            List<TaxonomyTree> tax = Session["Tree" + GUID] as List<TaxonomyTree>;
            TaxonomyTree res = tax.Where(g => g.Id.ToString() == ddlTaxonomy.SelectedValue).Single();
            this.UpdateTree(res.Tree);
        }

        private List<XElement> CreateTree(IEnumerable<AppFramework.Entities.TaxonomyItem> leafs)
        {
            List<XElement> rootElement = new List<XElement>();

            var roots = from g in leafs
                        where !g.ParentTaxonomyItemUid.HasValue
                        select g;
            foreach (var root in roots)
            {
                rootElement.Add(GetSubTree(root, leafs));
            }
            return rootElement;
        }

        private XElement GetSubTree(AppFramework.Entities.TaxonomyItem dataItem, IEnumerable<AppFramework.Entities.TaxonomyItem> leafs)
        {
            var item = new AppFramework.Core.Classes.TaxonomyItem(dataItem);
            XElement result = new XElement("taxonomy", new XAttribute("name", item.Name), new XAttribute("id", item.Id));
            result.Add((
                from g in leafs
                where
                g.ParentTaxonomyItemUid == item.Uid
                select GetSubTree(g, leafs)).ToList());
            return result;
        }
    }
}
