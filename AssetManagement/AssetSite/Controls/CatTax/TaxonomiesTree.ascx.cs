using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;

namespace AssetSite.Controls.CatTax
{
    public partial class TaxonomiesTree : System.Web.UI.UserControl
    {
        public delegate void TaxonomyItemSelectedHandler(TaxonomyItem item);

        public Taxonomy Taxonomy;


        /// <summary>
        /// Gets or sets the show checkboxes mode.
        /// </summary>
        /// <value>The show checkboxes.</value>
        public TreeNodeTypes ShowCheckboxes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the selected item uid.
        /// </summary>
        /// <value>The selected item uid.</value>
        public long SelectedItemUid
        {
            get
            {
                long uid = 0;
                long.TryParse(Taxonomies.SelectedValue, out uid);
                return uid;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether need to show Manage Asset types link
        /// </summary>
        /// <value><c>true</c> if show link; otherwise, <c>false</c>.</value>
        public bool ShowManageLink
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if 'Add node' link must be shown behind the node
        /// </summary>
        public bool ShowAddNodeLink
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Taxonomies.ShowCheckBoxes = this.ShowCheckboxes;
            if (!IsPostBack)
            {
                BuildTree();
            }
            Taxonomies.ExpandAll();
        }

        /// <summary>
        /// Builds the tree.
        /// </summary>
        public void BuildTree()
        {
            Taxonomies.Nodes.Clear();
            if (Taxonomy != null)
            {
                foreach (TaxonomyItem item in Taxonomy.RootItems)
                {
                    Taxonomies.Nodes.Add(item.ToTreeNode(ShowAddNodeLink, ShowManageLink));
                }
            }            
        }
    }
}